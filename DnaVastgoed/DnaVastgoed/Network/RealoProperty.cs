using DnaVastgoed.Models;
using RealoAPI;
using RealoAPI.Models;
using System.Collections.Generic;
using System.Linq;

namespace DnaVastgoed.Network {

    public class RealoProperty {

        private readonly DnaProperty _prop;

        public RealoProperty(DnaProperty prop) {
            _prop = prop;
        }

        /// <summary>
        /// Create and publish a new Realo listing.
        /// </summary>
        /// <param name="client">The Realo client</param>
        /// <param name="agencyId">The agency id</param>
        /// <returns>True if the property has been uploaded</returns>
        public bool Publish(RealoClient client, int agencyId) {
            Listing listing = new Listing(GetListingType(), GetListingWay());

            listing.Description = new Dictionary<string, string>() {
                { Language.NL.ToString(), _prop.Description }
            };

            listing.Address = new Address(Country.BE) {
                PostalCode = _prop.ZipCode
            };

            listing.EnergyConsumption = float.Parse(GetEnergy());
            listing.EnergyCertificateNumber = _prop.EPCNumber;
            listing.Price = int.Parse(_prop.Price);

            //Post the listing
            int listingId = client.Listings.Add(listing, agencyId);

            //Publish the new listing
            return client.Listings.Publish(listingId);
        }

        /// <summary>
        /// Get the proper type.
        /// </summary>
        /// <returns>The Realo listing type</returns>
        private ListingType GetListingType() {
            switch (_prop.Type) {
                case "Woning": return ListingType.HOUSE;
                case "Huis": return ListingType.HOUSE;
                case "Appartement": return ListingType.APARTMENT;
                case "Studio": return ListingType.APARTMENT;
                case "Assistentiewoning": return ListingType.MISCELLANEOUS;
                case "Industrieel/Commercieel": return ListingType.INDUSTRIAL;
                case "Grond": return ListingType.NEWBUILD_PROJECT;
                case "Garage": return ListingType.PARKING;
                case "Gemeubeld Appartement/Expats": return ListingType.APARTMENT;
            }

            return ListingType.MISCELLANEOUS;
        }

        /// <summary>
        /// Get the listing way using the status.
        /// </summary>
        /// <returns>The proper listing way</returns>
        private ListingWay GetListingWay() {
            return _prop.Status == "Te Koop" ? ListingWay.SALE : ListingWay.RENT;
        }

        /// <summary>
        /// Gets the energy score.
        /// </summary>
        /// <returns>The energy score</returns>
        private string GetEnergy() {
            return _prop.Energy.Split(" ")[0];
        }

        /// <summary>
        /// Create a new list of the right picture objects.
        /// </summary>
        /// <returns>An array of picture objects</returns>
        private ICollection<Picture> GetPictures() {
            ICollection<Picture> pictures = new List<Picture>();

            foreach (string imageUrl in _prop.Images.Select(img => img.Url)) {
                pictures.Add(new Picture(imageUrl));
            }

            return pictures;
        }
    }

}
