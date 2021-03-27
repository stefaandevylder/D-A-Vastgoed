﻿using DnaVastgoed.Models;
using ImmoVlanAPI;
using ImmoVlanAPI.Models;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;

namespace DnaVastgoed.Network {

    public class ImmoVlanProperty {

        private readonly DnaProperty _prop;

        public ImmoVlanProperty(DnaProperty prop) {
            _prop = prop;
        }

        /// <summary>
        /// Create and publish a new ImmoVlan property.
        /// </summary>
        /// <param name="client">The Immovlan client</param>
        /// <returns>The response</returns>
        public IRestResponse Publish(ImmoVlanClient client) {
            Property prop = new Property(_prop.Id, _prop.Id, CommercialStatus.ONLINE,
                new Classification(GetTransactionType(), GetPropertyType()),
                new Location(new Address(_prop.ZipCode)),
                new Description(_prop.Description, _prop.Description),
                new FinancialDetails(decimal.Parse(_prop.Price.Replace("€", "").Replace(".", "")), PriceType.AskedPrice)) {
                GeneralInformation = new GeneralInformation() {
                    ContactEmail = "info@dnavastgoed.be",
                    ContactPhone = "037761922"
                },
                Certificates = new Certificates() {
                    Epc = new EPC() {
                        EnergyConsumption = GetEnergy().GetValueOrDefault()
                    }
                },
                Attachments = new Attachments() {
                    Pictures = GetPictures(),
                }
            };

            return client.PublishProperty(prop).Result;
        }

        /// <summary>
        /// Gets the transaction type.
        /// </summary>
        /// <returns>An ImmoVlan transaction type</returns>
        private TransactionType GetTransactionType() {
            return _prop.Status == "Te Koop" ? TransactionType.SALE : TransactionType.RENT;
        }

        /// <summary>
        /// Gets the proper property type.
        /// </summary>
        /// <returns>An ImmoVlan property type</returns>
        private PropertyType GetPropertyType() {
            switch (_prop.Type) {
                case "Woning": return PropertyType.Residence;
                case "Huis": return PropertyType.Residence;
                case "Appartement": return PropertyType.FlatApartment;
                case "Studio": return PropertyType.FlatStudio;
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
        private int? GetEnergy() {
            if (_prop.Energy == null || _prop.Energy == "")
                return 0;
            return int.Parse(_prop.Energy.Split(" ")[0]);
        }

        /// <summary>
        /// Create a new list of the right picture objects.
        /// </summary>
        /// <returns>An array of picture objects</returns>
        private Picture[] GetPictures() {
            ICollection<Picture> pictures = new List<Picture>();

            for (int i = 0; i < _prop.Images.Count(); i++) {
                string imageUrl = _prop.Images.ToArray()[i].Url;

                pictures.Add(new Picture(i + 1, EncodeImage(imageUrl)));
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