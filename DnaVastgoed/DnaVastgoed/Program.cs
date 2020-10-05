using System;

namespace DnaVastgoed {

    class Program {

        private readonly string BASE_URL = "http://134.209.94.232/";
        private readonly string PROPERTIES = "/wp-json/wp/v2/property";
        private readonly string TYPES = "/wp-json/wp/v2/property_type";
        private readonly string STATUSES = "/wp-json/wp/v2/property_status";
        private readonly string LOCATIONS = "/wp-json/wp/v2/property_location";

        static void Main(string[] args) {
            Console.WriteLine("Hello World!");
        }
    }
}
