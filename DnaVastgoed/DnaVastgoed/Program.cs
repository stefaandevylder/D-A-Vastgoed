using Abot2.Crawler;
using Abot2.Poco;
using AngleSharp.Dom;
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

        private static readonly string BASE_URL = "http://134.209.94.232/wp-json/wp/v2/property?per_page=100&orderby=date";

        private static ICollection<string> _links = new List<string>();
        private static ICollection<Property> _properties = new List<Property>();

        static async Task Main(string[] args) {
            Console.WriteLine("Started parsing properties: " + BASE_URL);
            ParseJson();

            Console.WriteLine("Started crawling...");
            await StartCrawler();

            Console.WriteLine("Started database writeback...");
            UpdateDatabase();
        }

        /**
         * First we need to parse all possible properties 
         * from our wordpress Homeo theme. This uses
         * the BASE URL (wordpress json) and just retreives all
         * links. The reason we just cant use this for everything
         * is because the location is not within this information.
         * All custom fields are also not in this information.
         */
        private static void ParseJson() {
            HttpClient http = new HttpClient();
            var rawData = http.GetAsync(BASE_URL).Result.Content.ReadAsStringAsync().Result;
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
        private static async Task StartCrawler() {
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
        private static void PageCrawlCompleted(object sender, PageCrawlCompletedArgs e) {
            IHtmlDocument document = e.CrawledPage.AngleSharpHtmlDocument;

            Property property = new Property();
            property.ParseFromHTML(document);

            _properties.Add(property);
            Console.WriteLine("Received: " + property.ToString());
        }

        /**
         * Inserts all properties in the database and checks other versions,
         * if a new one is found, it has to be sent to the other
         * services. (Updates will come soon)
         */
        private static void UpdateDatabase() {
            using (var db = new ApplicationDbContext()) {
                db.Database.EnsureCreated();

                foreach (Property property in _properties) {
                    Property propertieFound = db.Properties.FirstOrDefault(x => x.Id == property.Id);

                    if (propertieFound != null) {
                        db.Add(property);
                        Console.WriteLine($"Added property {property.Id} to DB.");
                    }
                }

                db.SaveChanges();
            }
        }
    }
}
