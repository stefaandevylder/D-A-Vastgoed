using DnaVastgoed.Models;
using ImmoVlanAPI;
using ImmoVlanAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace DnaVastgoed.Network {

    public class ImmoVlanProperty : DnaProperty {

        private readonly string PROPERTY_PRO_ID = "02";
        private readonly string PROPERTY_SOFTWARE_ID = "02";

        /// <summary>
        /// Create and publish a new ImmoVlan property.
        /// </summary>
        /// <param name="client">The Immovlan client</param>
        public void CreateImmoVlan(ImmoVlanClient client) {
            Property prop = new Property(PROPERTY_PRO_ID, PROPERTY_SOFTWARE_ID, CommercialStatus.ONLINE,
                new Classification(GetTransactionType(), GetPropertyType(), true),
                new Location(new Address(ZipCode)),
                new Description(Description, Description),
                new FinancialDetails(decimal.Parse(Price))) {
                GeneralInformation = new GeneralInformation() {
                    ContactEmail = "info@dnavastgoed.be",
                    ContactPhone = "037761922"
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
                case "Woning": return PropertyType.Residence;
                case "Appartement": return PropertyType.FlatApartment;
                case "Assistentiewoning": return PropertyType.UnditerminedProperty;
                case "Industrieel/Commercieel": return PropertyType.CommerceBuilding;
                case "Grond": return PropertyType.DevelopmentSite;
                case "Garage": return PropertyType.GarageBuilding;
                case "Gemeubeld Appartement/Expats": return PropertyType.FlatApartment;
            }

            return PropertyType.UnditerminedProperty;
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
