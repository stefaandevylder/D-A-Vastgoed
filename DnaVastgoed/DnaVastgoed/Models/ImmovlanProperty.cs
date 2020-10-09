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
                new XElement("root",
                    new XElement("request", "",
                            new XAttribute("timestamp", DateTime.Now.ToString().Replace(" ", "T")),
                            new XAttribute("softwareId", $"{SoftwareId}"),
                        new XElement("action", "",
                            new XAttribute("proCustomerId", ProCustomerId),
                            new XAttribute("hashValidation", Guid.NewGuid().ToString("N")))),
                    new XElement("publish",
                        new XElement("property",
                            new XElement("classification")
                        )
                    )
                )
            );

            return doc;
        }
    }
}
