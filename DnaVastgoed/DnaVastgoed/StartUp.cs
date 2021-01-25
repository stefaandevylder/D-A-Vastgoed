using System.Threading.Tasks;

namespace DnaVastgoed {

    public class StartUp {
        
        /// <summary>
        /// Start the program.
        /// </summary>
        static async Task Main(string[] args) {
            await new Program().Start();
        }
    }
}
