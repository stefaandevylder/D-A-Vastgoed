using Abot2.Crawler;
using Abot2.Poco;
using AngleSharp.Html.Dom;
using DnaVastgoed.Data;
using DnaVastgoed.Models;
using DnaVastgoed.Network;
using ImmoVlanAPI;
using Newtonsoft.Json.Linq;
using RealoAPI;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace DnaVastgoed {

    public class Program {

        private readonly string BASE_URL = "http://134.209.94.232/wp-json/wp/v2/property?per_page=100&orderby=date";
        private ICollection<string> _links = new List<string>();

        private readonly PropertyRepository _repo;
        private readonly ImmoVlanClient _immovlanClient;
        private readonly RealoClient _realoClient;

        public Program() {
            ApplicationDbContext context = new ApplicationDbContext();

            _repo = new PropertyRepository(context);

            _immovlanClient = new ImmoVlanClient("", "", 1, "", true);
            _realoClient = new RealoClient("", "");
        }

        /// <summary>
        /// Parse the properties from the base url, create a database
        /// and start crawling the webpages for all extra information.
        /// </summary>
        public async Task Start() {
            Console.WriteLine("Started parsing properties: " + BASE_URL);
            ParseJson(BASE_URL);

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

            Property property = new Property();
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
        private void AddOrUpdateProperty(Property property) {
            Property propertyFound = _repo.Get(property.Id);

            if (propertyFound == null) {
                _repo.Add(property);
                _repo.SaveChanges();

                // I know this seems weird but ill fix this once I have time
                ((ImmoVlanProperty) property).CreateImmoVlan(_immovlanClient);
                ((RealoProperty) property).CreateRealo(_realoClient, 1);

                Console.WriteLine($"Added property {property.Id} to database, Immovlan & Realo.");
            } else {
                if (property.Equals(propertyFound)) {
                    Console.WriteLine($"Property {property.Id} already exists, skipping.");
                } else {
                    _repo.Remove(propertyFound);
                    _repo.Add(property);
                    _repo.SaveChanges();

                    Console.WriteLine($"Property {property.Id} exists, but has been updated.");
                }
            }
        }
    }
}
