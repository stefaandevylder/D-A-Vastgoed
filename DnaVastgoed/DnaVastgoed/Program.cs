using Abot2.Crawler;
using Abot2.Poco;
using AngleSharp.Html.Dom;
using DnaVastgoed.Data;
using DnaVastgoed.Models;
using DnaVastgoed.Network;
using ImmoVlanAPI;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using RealoAPI;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace DnaVastgoed {

    public class Program {

        private readonly bool STAGING = true;
        private IConfiguration Configuration;

        private ICollection<string> _links = new List<string>();

        private readonly PropertyRepository _repo;
        private readonly ImmoVlanClient _immovlanClient;
        private readonly RealoClient _realoClient;

        public Program() {
            Configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddUserSecrets<Program>()
                .Build();

            ApplicationDbContext context = new ApplicationDbContext();
            _repo = new PropertyRepository(context);

            _immovlanClient = new ImmoVlanClient(Configuration["ImmoVlan:BusinessEmail"],
                Configuration["ImmoVlan:TechincalEmail"], int.Parse(Configuration["ImmoVlan:SoftwareId"]), 
                Configuration["ImmoVlan:ProCustomerId"], Configuration["ImmoVlan:SoftwarePassword"], STAGING);
            _realoClient = new RealoClient(Configuration["Realo:PublicKey"], Configuration["Realo:PrivateKey"], STAGING);
        }

        /// <summary>
        /// Parse the properties from the base url, create a database
        /// and start crawling the webpages for all extra information.
        /// </summary>
        public async Task Start() {
            Console.WriteLine("Started parsing properties: " + Configuration["BaseURL"]);
            ParseJson(Configuration["BaseURL"]);

            Console.WriteLine("Creating database if needed...");
            await _repo.CreateDatabase();

            Console.WriteLine("Started crawling...");
            await StartCrawler();
        }

        /// <summary>
        /// First we need to parse all possible properties 
        /// from our wordpress Homeo theme.This uses
        /// the BASE URL(wordpress json) and just retreives all
        /// links.The reason we just cant use this for everything
        /// is because the location is not within this information.
        ///
        /// All custom fields are also not in this information.
        /// </summary>
        /// <param name="baseUrl">The base URL we are going to parse</param>
        private void ParseJson(string baseUrl) {
            HttpClient http = new HttpClient();
            var rawData = http.GetAsync(baseUrl).Result.Content.ReadAsStringAsync().Result;
            var myJson = JArray.Parse(rawData);

            foreach (JObject item in myJson) {
                _links.Add(item["link"].ToString());
                Console.WriteLine($"Link added: {item["link"]}");
            }
        }

        /// <summary>
        /// Starts the crawler to crawl found pages.
        /// It takes 1 second per page to ensure it does not lock
        /// us out and we can keep crawling the site?
        /// </summary>
        private async Task StartCrawler() {
            var config = new CrawlConfiguration {
                MaxPagesToCrawl = 1,
                MinCrawlDelayPerDomainMilliSeconds = 100
            };

            var crawler = new PoliteWebCrawler(config);
            crawler.PageCrawlCompleted += PageCrawlCompleted;

            foreach (string link in _links) {
                crawler = new PoliteWebCrawler(config);
                crawler.PageCrawlCompleted += PageCrawlCompleted;

                await crawler.CrawlAsync(new Uri(link));
            }
        }

        /// <summary>
        /// When a page is done crawling, we'll need the information
        /// regarding the properties so we can save them.This exctracts
        /// the information through html.
        /// </summary>
        /// <param name="sender">The sender object</param>
        /// <param name="e">Any arguments from the completed crawl</param>
        private void PageCrawlCompleted(object sender, PageCrawlCompletedArgs e) {
            IHtmlDocument document = e.CrawledPage.AngleSharpHtmlDocument;

            DnaProperty property = new DnaProperty();
            property.ParseFromHTML(document);

            Console.WriteLine("Found: " + property.ToString());

            AddOrUpdateProperty(property);
        }

        /// <summary>
        /// Adds property in the database and checks other versions,
        /// if a new one is found, it has to be sent to the other
        /// services. (Updates will come soon)
        /// </summary>
        /// <param name="property">The property to insert</param>
        private void AddOrUpdateProperty(DnaProperty property) {
            DnaProperty propertyFound = _repo.Get(property.Name);

            if (property.Price != null) {
                if (propertyFound == null) {
                    _repo.Add(property);
                    _repo.SaveChanges();

                    var result = new ImmoVlanProperty(property).Publish(_immovlanClient);
                    Console.WriteLine(result.Content);

                    // Realo is not active, it has not been paid for by this client.
                    // new RealoProperty(property).Publish(_realoClient, 1);

                    Console.WriteLine($"Added property {property.Name} to database & Immovlan.");
                } else {
                    if (!property.Equals(propertyFound)) {
                        _repo.Remove(propertyFound);
                        _repo.Add(property);
                        _repo.SaveChanges();

                        new ImmoVlanProperty(property).Publish(_immovlanClient);

                        Console.WriteLine($"Property {property.Name} exists, but has been updated.");
                    } else {
                        Console.WriteLine($"Property {property.Name} already exists, skipping.");
                    }
                }
            } else {
                Console.WriteLine($"Property {property.Name} has price null, can't add to DB.");
            }
        }
    }
}
