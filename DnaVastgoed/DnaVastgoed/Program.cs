using Abot2.Crawler;
using Abot2.Poco;
using AngleSharp.Html.Dom;
using DnaVastgoed.Data;
using DnaVastgoed.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace DnaVastgoed {

    class Program {

        private ApplicationDbContext _database = new ApplicationDbContext();
        private ICollection<string> _links = new List<string>();

        /**
         * Starts the program.
         */
        public async Task Start(string baseUrl) {
            Console.WriteLine("Started parsing properties: " + baseUrl);
            ParseJson(baseUrl);

            Console.WriteLine("Creating database if needed...");
            await _database.Database.EnsureCreatedAsync();

            Console.WriteLine("Started crawling...");
            await StartCrawler();
        }

        /**
         * First we need to parse all possible properties 
         * from our wordpress Homeo theme. This uses
         * the BASE URL (wordpress json) and just retreives all
         * links. The reason we just cant use this for everything
         * is because the location is not within this information.
         * All custom fields are also not in this information.
         */
        private void ParseJson(string baseUrl) {
            HttpClient http = new HttpClient();
            var rawData = http.GetAsync(baseUrl).Result.Content.ReadAsStringAsync().Result;
            var myJson = JArray.Parse(rawData);

            foreach (JObject item in myJson) {
                _links.Add(item["link"].ToString());
                Console.WriteLine($"Link added: {item["link"]}");
            }
        }

        /**
         * Starts the crawler to crawl found pages.
         * It takes 1 second per page to ensure it does not lock
         * us out and we can keep crawling the site?
         */
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

        /**
         * When a page is done crawling, we'll need the information
         * regarding the properties so we can save them. This exctracts
         * the information through html.
         */
        private void PageCrawlCompleted(object sender, PageCrawlCompletedArgs e) {
            IHtmlDocument document = e.CrawledPage.AngleSharpHtmlDocument;

            Property property = new Property();
            property.ParseFromHTML(document);

            Console.WriteLine("Received: " + property.ToString());

            InsertProperty(property);
        }

        /**
         * Inserts property in the database and checks other versions,
         * if a new one is found, it has to be sent to the other
         * services. (Updates will come soon)
         */
        private void InsertProperty(Property property) {
            Property propertyFound = _database.Properties.FirstOrDefault(p => p == property);

            if (propertyFound == null) {
                _database.Properties.Add(property);
                Console.WriteLine($"Added property {property.Id} to database.");

                _database.SaveChanges();
            } else {
                Console.WriteLine($"Property {property.Id} already exists, skipping.");
            }
        }
    }
}
