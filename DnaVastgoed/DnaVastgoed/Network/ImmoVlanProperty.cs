using ImmoVlanAPI;
using ImmoVlanAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace DnaVastgoed.Network {

    public class ImmoVlanProperty : NetworkProperty {

        /// <summary>
        /// Create and publish a new ImmoVlan property.
        /// </summary>
        /// <param name="client">The Immovlan client</param>
        public void CreateImmoVlan(ImmoVlanClient client) {
            Property prop = new Property("123", "123", CommercialStatus.ONLINE,
                new Classification(GetTransactionType(), GetPropertyType(), true),
                new Location(new Address(GetZipCode().ToString())),
                new Description(Description, Description),
                new FinancialDetails(decimal.Parse(Price))) {
                GeneralInformation = new GeneralInformation() {
                    ContactEmail = "sdhjgfj@fjhk.com"
                },
                Certificates = new Certificates() {
                    Epc = new EPC() {
                        Reference = EPCNumber,
                        EnergyConsumption = GetEnergy()
                    }
                },
                Attachments = new Attachments() {
                    Pictures = GetPictures(),
                }
            };

            client.PublishProperty(prop);
        }

        /// <summary>
        /// Gets the transaction type.
        /// </summary>
        /// <returns>An ImmoVlan transaction type</returns>
        private TransactionType GetTransactionType() {
            return Status == "Te Koop" ? TransactionType.SALE : TransactionType.RENT;
        }

        /// <summary>
        /// Gets the proper property type.
        /// </summary>
        /// <returns>An ImmoVlan property type</returns>
        private PropertyType GetPropertyType() {
            switch (Type) {
                case "Villa": return PropertyType.Villa;
                case "Woning": return PropertyType.Residence;
                case "Appartement": return PropertyType.FlatApartment;
                case "Assistentiewoning": return PropertyType.UnditerminedProperty;
                case "Industrieel/Commercieel": return PropertyType.CommerceBuilding;
                case "Grond/Buitenparking": return PropertyType.DevelopmentSite;
                case "Garage": return PropertyType.GarageBuilding;
            }

            return PropertyType.UnditerminedProperty;
        }

        /// <summary>
        /// Get the zipcode from a location.
        /// </summary>
        /// <returns>The zipcode</returns>
        private int GetZipCode() {
            if (Location.Contains("Beveren")) return 9120;
            if (Location.Contains("Kemzeke")) return 9190;
            if (Location.Contains("Nieuwkerken-Waas")) return 9100;
            if (Location.Contains("Sint-Gillis-Waas")) return 9170;
            if (Location.Contains("Sint-Niklaas")) return 9100;
            if (Location.Contains("Vrasene")) return 9120;

            return 9100;
        }

        /// <summary>
        /// Gets the energy score.
        /// </summary>
        /// <returns>The energy score</returns>
        private int GetEnergy() {
            return int.Parse(Energy.Split(" ")[0]);
        }

        /// <summary>
        /// Create a new list of the right picture objects.
        /// </summary>
        /// <returns>An array of picture objects</returns>
        private Picture[] GetPictures() {
            ICollection<Picture> pictures = new List<Picture>();

            for (int i = 1; i <= Images.Count(); i++) {
                string imageUrl = Images.ToArray()[i].Url;

                pictures.Add(new Picture(i, EncodeImage(imageUrl)));
            }

            return pictures.ToArray();
        }

        /// <summary>
        /// Encode an image from the web to a base64.
        /// </summary>
        /// <param name="imageUrl">The url we need to encode</param>
        /// <returns>A base64 in string form</returns>
        private string EncodeImage(string imageUrl) {
            using (WebClient webClient = new WebClient()) {
                byte[] data = webClient.DownloadData(imageUrl);

                return Convert.ToBase64String(data);
            }
        }

    }

}
