using AngleSharp.Dom;
using AngleSharp.Html.Dom;

namespace DnaVastgoed.Models {

    class Property {

        public string Id { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public string Location { get; private set; }
        public string Energy { get; private set; }
        public string Price { get; private set; }
        public string LotArea { get; private set; }
        public string LivingArea { get; private set; }
        public string Rooms { get; private set; }
        public string Bedrooms { get; private set; }
        public string Bathrooms { get; private set; }
        public string Status { get; private set; }
        public string EPCNumber { get; private set; }

        //Belgian Law Things
        public string Katastraalinkomen { get; private set; }
        public string OrientatieAchtergevel { get; private set; }
        public string Elektriciteitskeuring { get; private set; }
        public string Bouwvergunning { get; private set; }
        public string StedenbouwkundigeBestemming { get; private set; }
        public string Verkavelingsvergunning { get; private set; }
        public string Dagvaarding { get; private set; }
        public string Verkooprecht { get; private set; }
        public string RisicoOverstroming { get; private set; }
        public string AfgebakendOverstromingsGebied { get; private set; }

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

        private string GetText(IHtmlDocument document, string querySelector) {
            return document.QuerySelector(querySelector) != null ? document.QuerySelector(querySelector).Text() : "";
        }

        public override string ToString() {
            return $"Property {Name} with ID: {Id}";
        }
    }
}
