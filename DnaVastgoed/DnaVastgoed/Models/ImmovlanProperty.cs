using System;
using System.Xml.Linq;

namespace DnaVastgoed.Models {

    class ImmovlanProperty : Property {

        //Settings for ImmoVlan
        public readonly string API = "http://api.staging.immo.vlan.be/upload";
        public readonly int SoftwareId = 2;
        public readonly string ProCustomerId = "XXXXXX";

        /**
         * Creates an XML document especially designed
         * for the ImmoVlan API. Information in this XML
         * only contains the required options.
         * The required items can be found on: http://api.immo.vlan.be/Files/XmlTransferXsd
         */
        public XDocument ToXMLDocument() {
            XDocument doc = new XDocument(
                new XElement("request", 
                new XAttribute("timestamp", DateTime.Now.ToString().Replace(" ", "T")), 
                new XAttribute("softwareId", $"{SoftwareId}"),
                    new XElement("action", 
                    new XAttribute("proCustomerId", ProCustomerId), 
                    new XAttribute("hashValidation", Guid.NewGuid().ToString("N")),
                        new XElement("publish",
                            new XElement("property",
                            new XAttribute("propertyProId", Id),
                            new XAttribute("propertySoftwareId", Id),
                            new XAttribute("commercialStatus", CommercialStatus(Price)),
                                new XElement("classification",
                                    new XElement("transactionType", TransactionType(Status)),
                                    new XElement("propertyTypeId", TypeToInt(Type))
                                )
                            ),
                            new XElement("location")
                        )
                    )
                )
            );

            return doc;
        }

        /**
         * A property is sold when the price has been removed.
         */
        private string CommercialStatus(string price) {
            return price == "" ? "SOLD" : "ONLINE";
        }

        /**
         * Converts the status to a status that Immovlan accepts.
         */
        private string TransactionType(string status) {
            return status == "Te Koop" ? "SALE" : "RENT";
        }

        /**
         * Converts the type to an int that Immovlan accepts.
         */
        private int TypeToInt(string type) {
            switch (type) {
                case "Villa": return 20;
                case "Woning": return 10;
                case "Appartement": return 170;
                case "Assistentiewoning": return 130;
                case "Industrieel/Commercieel": return 320;
                case "Grond/Buitenparking": return 540;
                case "Garage": return 550;
            }

            return 0;
        }
    }
}
