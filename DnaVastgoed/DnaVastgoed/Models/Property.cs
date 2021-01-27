using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace DnaVastgoed.Models {

    public class Property {

        //Identification
        public string Id { get; private set; }
        public int ImmovlanId { get; set; }
        public int RealoId { get; set; }
        public DateTime LastUpdated { get; set; }

        //Basic information
        public ICollection<PropertyImage> Images { get; set; }
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


        /// <summary>
        /// Parses the HTML document to an entity.
        /// 
        /// ATTENTION: If the front-site changes, this should change too.
        /// </summary>
        /// <param name="document">The document we need to parse</param>
        public void ParseFromHTML(IHtmlDocument document) {
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
                    case "Pand ID": Id = value; break;
                    case "Grondoppervlakte": LotArea = value; break;
                    case "Oppervlakte bewoonbaar": LivingArea = value; break;
                    case "Kamers": Rooms = value; break;
                    case "Slaapkamers": Bedrooms = value; break;
                    case "Badkamers": Bathrooms = value; break;
                    case "Prijs": Price = value; break;
                    case "Pand Status": Status = value;  break;
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

            Images = new List<PropertyImage>();
            IHtmlCollection<IElement> images = document.QuerySelectorAll("div.list-gallery-property-v2 div.image-wrapper img");

            foreach (var el in images) {
                Images.Add(new PropertyImage() {
                    Url = el.GetAttribute("data-src")
                });
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

    public class PropertyImage {

        public int Id { get; set; }
        public string Url { get; set; }

    }
}
