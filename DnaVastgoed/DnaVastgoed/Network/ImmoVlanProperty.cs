﻿using DnaVastgoed.Models;
using ImmoVlanAPI;
using ImmoVlanAPI.Models;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

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
            Property prop = new Property(_prop.Id, _prop.Id, GetCommercialStatus(),
                new Classification(GetTransactionType(), GetPropertyType()),
                new Location(GetLocation()) {
                    IsAddressDisplayed = true
                },
                new Description(_prop.Description, _prop.Description),
                new FinancialDetails(GetPrice(), PriceType.AskedPrice)) {
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
        /// Gets the commercial status of a property.
        /// </summary>
        /// <returns>The correct status</returns>
        private CommercialStatus GetCommercialStatus() {
            return _prop.Status.Contains("Verkocht") || _prop.Status.Contains("Verhuurd") || _prop.Status.Contains("Realisatie") ? CommercialStatus.SOLD : CommercialStatus.ONLINE;
        }

        /// <summary>
        /// Gets the transaction type.
        /// </summary>
        /// <returns>An ImmoVlan transaction type</returns>
        private TransactionType GetTransactionType() {
            return _prop.Status.Contains("Verkocht") || _prop.Status.Contains("Te Koop") ? TransactionType.SALE : TransactionType.RENT;
        }

        /// <summary>
        /// Get the location already parsed in an adress.
        /// </summary>
        /// <returns>The correct address object</returns>
        private Address GetLocation() {
            string[] streetAndCity = _prop.Location.Split(", ");

            string[] streetAndNumber = streetAndCity[0].Split(" ");
            string[] zipAndCity = streetAndCity[1].Split(" ");

            return new Address(zipAndCity[0], streetAndNumber[0], streetAndNumber[1], null, zipAndCity[1]);
        }

        /// <summary>
        /// Get the price of the property.
        /// </summary>
        /// <returns>The decimal price form</returns>
        private decimal GetPrice() {
            if (string.IsNullOrEmpty(_prop.Price) || !_prop.Price.Contains("€")) return 0;

            return decimal.Parse(_prop.Price.Replace("€", "").Replace(".", ""));
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
                case "Opbrengsteigendom": return PropertyType.ReturnBuilding;
                case "Landhuis": return PropertyType.Villa;
                case "Bouwgrond": return PropertyType.DevelopmentSite;
            }

            return PropertyType.UnditerminedProperty;
        }

        /// <summary>
        /// Gets the energy score.
        /// </summary>
        /// <returns>The energy score</returns>
        private int? GetEnergy() {
            if (_prop.Energy == null || _prop.Energy == "") return 0;

            return int.Parse(_prop.Energy.Split(" ")[0]);
        }

        /// <summary>
        /// Create a new list of the right picture objects.
        /// </summary>
        /// <returns>An array of picture objects</returns>
        private Picture[] GetPictures() {
            ICollection<Picture> pictures = new List<Picture>();

            for (int i = 0; i < _prop.Images.Take(31).Count(); i++) {
                string imageUrl = _prop.Images.ToArray()[i].Url;

                pictures.Add(new Picture(i + 1, EncodeImage(imageUrl)));
            }

            Console.WriteLine($"Pictures encoded: {pictures.Count()}");

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
                string baseData = Convert.ToBase64String(data);

                return baseData;
            }
        }

    }

}
