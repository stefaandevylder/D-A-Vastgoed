using AngleSharp.Dom;
using AngleSharp.Html.Dom;

namespace DnaVastgoed.Models {

    class Property {

        //Identification
        public string Id { get; private set; }
        public string Name { get; set; }
        public bool IsUploaded { get; set; }

        //Basic information
        public string Description { get; set; }
        public string Location { get; set; }
        public string Energy { get; set; }
        public string Price { get; set; }
        public string LotArea { get; set; }
        public string LivingArea { get; set; }
        public string Rooms { get; set; }
        public string Bedrooms { get; set; }
        public string Bathrooms { get; set; }
        public string Status { get; set; }
        public string EPCNumber { get; set; }

        //Necessary items for Belgian law
        public string Katastraalinkomen { get; set; }
        public string OrientatieAchtergevel { get; set; }
        public string Elektriciteitskeuring { get; set; }
        public string Bouwvergunning { get; set; }
        public string StedenbouwkundigeBestemming { get; set; }
        public string Verkavelingsvergunning { get; set; }
        public string Dagvaarding { get; set; }
        public string Verkooprecht { get; set; }
        public string RisicoOverstroming { get; set; }
        public string AfgebakendOverstromingsGebied { get; set; }

        /**
         * Parses the HTML document to an entity.
         * 
         * ATTENTION: If the front-site changes, this should change too.
         */
        public void ParseFromHTML(IHtmlDocument document) {
            Name = GetText(document, "h1.property-title");
            Description = GetText(document, "div.description-inner");
            Location = GetText(document, "div.property-location a");
            Energy = GetText(document, "div.indicator-energy");

            IHtmlCollection<IElement> detailsList = document.QuerySelectorAll("div.property-detail-detail ul li");

            foreach (var el in detailsList) {
                string key = el.QuerySelector("div.text").Text().TrimEnd(':');
                string value = el.QuerySelector("div.value").Text();

                switch (key) {
                    case "Pand ID": Id = value; break;
                    case "Grondoppervlakte": LotArea = value; break;
                    case "Oppervlakte bewoonbaar": LivingArea = value; break;
                    case "Kamers": Rooms = value; break;
                    case "Slaapkamers": Bedrooms = value; break;
                    case "Badkamers": Bathrooms = value; break;
                    case "Prijs": Price = value; break;
                    case "Pand Status": Status = value; break;
                    case "EPC Certificaatnr": EPCNumber = value; break;
                    case "Katastraal Inkomen (KI)": Katastraalinkomen = value; break;
                    case "Orientatie achtergevel": OrientatieAchtergevel = value; break;
                    case "Elektriciteitskeuring": Elektriciteitskeuring = value; break;
                    case "Bouwvergunning": Bouwvergunning = value; break;
                    case "Stedenbouwkundige bestemming": StedenbouwkundigeBestemming = value; break;
                    case "Verkavelingsvergunning": Verkavelingsvergunning = value; break;
                    case "Dagvaarding": Dagvaarding = value; break;
                    case "Verkooprecht": Verkooprecht = value; break;
                    case "Risicozone voor overstromingen": RisicoOverstroming = value; break;
                    case "Afgebakend overstromingsgebied": AfgebakendOverstromingsGebied = value; break;
                }
            }
        }

        /**
         * To see if a detail exists on the page.
         */
        private string GetText(IHtmlDocument document, string querySelector) {
            return document.QuerySelector(querySelector) != null ? document.QuerySelector(querySelector).Text() : "";
        }

        /**
         * Needed for console writing.
         */
        public override string ToString() {
            return $"Property {Name} with ID: {Id}";
        }
    }
}
