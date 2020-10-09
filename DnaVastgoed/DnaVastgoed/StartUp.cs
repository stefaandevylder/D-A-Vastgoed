using System.Threading.Tasks;

namespace DnaVastgoed {

    class StartUp {
        
        //URL with a list (JSON) of all properties on that site.
        private static readonly string BASE_URL = "http://134.209.94.232/wp-json/wp/v2/property?per_page=100&orderby=date";

        static async Task Main(string[] args) {
            await new Program().Start(BASE_URL);
        }
    }
}
