using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;

namespace DnaVastgoed.Models {

    public class DnaProperty {

        //Identification
        [Key]
        public string Id { get; private set; }
        public string ImmovlanId { get; set; }
        public string RealoId { get; set; }

        //Basic information
        public string Name { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public string Energy { get; set; }
        public string Price { get; set; }
        public string LotArea { get; set; }
        public string LivingArea { get; set; }
        public string Rooms { get; set; }
        public string Bedrooms { get; set; }
        public string Bathrooms { get; set; }
        public string EPCNumber { get; set; }
        public ICollection<DnaPropertyImage> Images { get; set; }

        //Necessary items for Belgian law
        public string KatastraalInkomen { get; set; }
        public string OrientatieAchtergevel { get; set; }
        public string Elektriciteitskeuring { get; set; }
        public string Bouwvergunning { get; set; }
        public string StedenbouwkundigeBestemming { get; set; }
        public string Verkavelingsvergunning { get; set; }
        public string Dagvaarding { get; set; }
        public string Verkooprecht { get; set; }
        public string RisicoOverstroming { get; set; }
        public string AfgebakendOverstromingsGebied { get; set; }

        public DnaProperty() {
            Id = (DateTime.Now.Ticks / TimeSpan.TicksPerSecond).ToString();

            Images = new List<DnaPropertyImage>();
        }

        /// <summary>
        /// Parses the HTML document to an entity.
        /// 
        /// ATTENTION: If the front-site changes, this should change too.
        /// </summary>
        /// <param name="document">The document we need to parse</param>
        /// <param name="replaceUrl">The old url to replace</param>
        /// <param name="baseUrl">The new base url</param>
        public void ParseFromHTML(IHtmlDocument document, string replaceUrl, string baseUrl) {
            Name = GetText(document, "h1.property-title");
            Description = GetText(document, "div.description-inner");
            Location = GetText(document, "div.property-location a");
            Energy = GetText(document, "div.indicator-energy");
            Type = GetText(document, "a.type-property");

            IHtmlCollection<IElement> detailsList = document.QuerySelectorAll("div.property-detail-detail ul li");

            foreach (var el in detailsList) {
                string key = GetText(el, "div.text").TrimEnd(':');
                string value = GetText(el, "div.value");

                switch (key) {
                    case "Grondoppervlakte": LotArea = value; break;
                    case "Oppervlakte bewoonbaar": LivingArea = value; break;
                    case "Kamers": Rooms = value; break;
                    case "Slaapkamers": Bedrooms = value; break;
                    case "Badkamers": Bathrooms = value; break;
                    case "Prijs": Price = value; break;
                    case "Pand Status": Status = value; break;
                    case "EPC Certificaatnr": EPCNumber = value; break;
                    case "Katastraal Inkomen (KI)": KatastraalInkomen = value; break;
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

            IHtmlCollection<IElement> images = document.QuerySelectorAll("div.list-gallery-property-v2 div a");

            foreach (var el in images) {
                Images.Add(new DnaPropertyImage(el.GetAttribute("href").Replace(replaceUrl, baseUrl)));
            }
        }

        /// <summary>
        /// Get text from an element if existant.
        /// </summary>
        /// <param name="doc">The document we need to select from</param>
        /// <param name="querySelector">The query selector</param>
        private string GetText(IHtmlDocument doc, string querySelector) {
            return doc.QuerySelector(querySelector) != null ? Regex.Replace(doc.QuerySelector(querySelector).Text(), @"^\s+|\s+$|\s+(?=\s)", "") : "";
        }

        /// <summary>
        /// Get text from an element if existant.
        /// </summary>
        /// <param name="el">The element we need to select from</param>
        /// <param name="querySelector">The query selector</param>
        private string GetText(IElement el, string querySelector) {
            return el.QuerySelector(querySelector) != null ? Regex.Replace(el.QuerySelector(querySelector).Text(), @"^\s+|\s+$|\s+(?=\s)", "") : "";
        }

        /// <summary>
        /// Compare two properties and check if they are the same.
        /// </summary>
        /// <param name="p">The property to check</param>
        /// <returns>True if they are equal</returns>
        public bool Equals(DnaProperty p) {
            return p.Name == Name
                && p.Type == Type
                && p.Status == Status
                && p.Description == Description
                && p.Location == Location
                && p.Energy == Energy
                && p.Price == Price
                && p.LotArea == LotArea
                && p.LivingArea == LivingArea
                && p.Rooms == Rooms
                && p.Bedrooms == Bedrooms
                && p.Bathrooms == Bathrooms
                && p.EPCNumber == EPCNumber
                && p.KatastraalInkomen == KatastraalInkomen
                && p.OrientatieAchtergevel == OrientatieAchtergevel
                && p.Elektriciteitskeuring == Elektriciteitskeuring
                && p.StedenbouwkundigeBestemming == StedenbouwkundigeBestemming
                && p.Verkavelingsvergunning == Verkavelingsvergunning
                && p.Dagvaarding == Dagvaarding
                && p.Verkooprecht == Verkooprecht
                && p.RisicoOverstroming == RisicoOverstroming
                && p.AfgebakendOverstromingsGebied == AfgebakendOverstromingsGebied;
        }

        /// <summary>
        /// Returns a small string of the property.
        /// </summary>
        /// <returns>The property with only a few properties</returns>
        public string ToSmallString() {
            return $"Property {Name} with ID: {Id}" +
                $"\nLoc: {Location}" +
                $"\nAmount of images: {Images.Count()}";
        }

        /// <summary>
        /// Needed for console writing.
        /// </summary>
        /// <returns>The property with the right ID</returns>
        public override string ToString() {
            return $"Property {Name} with ID: {Id}" +
                $"\nType: {Type}" +
                $"\nStatus: {Status}" +
                $"\nDesc: {Description}" +
                $"\nLoc: {Location}" +
                $"\nEnergy: {Energy}" +
                $"\nPrice: {Price}" +
                $"\nLot area: {LotArea}" +
                $"\nLiving area: {LivingArea}" +
                $"\nRooms: {Rooms}" +
                $"\nBedrooms: {Bedrooms}" +
                $"\nEPC nr: {EPCNumber}" +
                $"\nKI: {KatastraalInkomen}" +
                $"\nOrientatie achtergevel: {OrientatieAchtergevel}" +
                $"\nElektriciteitskeuring: {Elektriciteitskeuring}" +
                $"\nBouwvergunning: {Bouwvergunning}" +
                $"\nStedenbouwkundig: {StedenbouwkundigeBestemming}" +
                $"\nVerkavelings: {Verkavelingsvergunning}" +
                $"\nDagvaarding: {Dagvaarding}" +
                $"\nVerkooprecht: {Verkooprecht}" +
                $"\nOverstroming: {RisicoOverstroming}" +
                $"\nAfgebakend: {AfgebakendOverstromingsGebied}" +
                $"\nImages: {string.Join(",", Images.Select(i => i.Url))}";
        }

    }

}
