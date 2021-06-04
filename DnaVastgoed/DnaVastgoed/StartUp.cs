using System;
using System.Threading.Tasks;

namespace DnaVastgoed {

    public class StartUp {
        
        /// <summary>
        /// Start the program.
        /// 
        /// Optional arguments:
        /// STAGING (Wether to use the staging or not)
        /// </summary>
        static async Task Main(string[] args) {
            bool isStaging = args.Length > 0 ? args[0].ToUpper().Equals("STAGING") : false;

            if (isStaging)
                Console.WriteLine("Staging mode is active.");

            await new Program(isStaging).Start();
        }
    }
}
