using Abot2.Crawler;
using Abot2.Poco;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace DnaVastgoed {

    class Program {

        private static readonly string BASE_URL = "http://134.209.94.232/wp-json/wp/v2/property?per_page=100&orderby=date";

        private static ICollection<string> _links = new List<string>();

        static async Task Main(string[] args) {
            Console.WriteLine("Started parsing properties: " + BASE_URL);
            ParseJson();

            Console.WriteLine("Started crawling.");
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
                MinCrawlDelayPerDomainMilliSeconds = 1000
            };

            var crawler = new PoliteWebCrawler(config);
            crawler.PageCrawlCompleted += PageCrawlCompleted;
            
            foreach (string link in _links) {
                crawler = new PoliteWebCrawler(config);
                crawler.PageCrawlCompleted += PageCrawlCompleted;

                await crawler.CrawlAsync(new Uri(link));
            }
        }

        private static void PageCrawlCompleted(object sender, PageCrawlCompletedArgs e) {
            IHtmlDocument document = e.CrawledPage.AngleSharpHtmlDocument;

            string name = GetText(document, "h1.property-title");
            string desc = GetText(document, "div.description-inner");
            string energy = GetText(document, "div.indicator-energy");
            string location = GetText(document, "div.property-location a");
            IHtmlCollection<IElement> detailsList = document.QuerySelectorAll("div.property-detail-detail ul li");

            Console.WriteLine(name);
            Console.WriteLine(desc);
            Console.WriteLine(energy);
            Console.WriteLine(location);

            foreach (var el in detailsList) {
                Console.WriteLine(el.QuerySelector("div.text").Text() + el.QuerySelector("div.value").Text());
            }
        }

        private static string GetText(IHtmlDocument document, string querySelector) {
            return document.QuerySelector(querySelector) != null ? document.QuerySelector(querySelector).Text() : "";
        }
    }
}
