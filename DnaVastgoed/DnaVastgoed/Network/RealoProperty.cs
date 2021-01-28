using RealoAPI;
using RealoAPI.Models;
using System.Collections.Generic;
using System.Linq;

namespace DnaVastgoed.Network {

    public class RealoProperty : NetworkProperty {

        /// <summary>
        /// Create and publish a new Realo listing.
        /// </summary>
        /// <param name="client">The Realo client</param>
        /// <param name="agencyId">The agency id</param>
        public void CreateRealo(RealoClient client, int agencyId) {
            Listing listing = new Listing(GetListingType(), GetListingWay());

            listing.Description = new Dictionary<string, string>() {
                { Language.NL.ToString(), Description }
            };

            listing.Address = new Address(Country.BE) {
                PostalCode = GetZipCode().ToString()
            };

            listing.EnergyConsumption = float.Parse(GetEnergy());
            listing.EnergyCertificateNumber = EPCNumber;
            listing.Price = int.Parse(Price);

            //Post the listing
            int listingId = client.Listings.Add(listing, agencyId);

            //Publish the new listing
            client.Listings.Publish(listingId);
        }

        /// <summary>
        /// Get the proper type.
        /// </summary>
        /// <returns>The Realo listing type</returns>
        private ListingType GetListingType() {
            switch (Type) {
                case "Villa": return ListingType.HOUSE;
                case "Woning": return ListingType.HOUSE;
                case "Appartement": return ListingType.APARTMENT;
                case "Assistentiewoning": return ListingType.MISCELLANEOUS;
                case "Industrieel/Commercieel": return ListingType.INDUSTRIAL;
                case "Grond/Buitenparking": return ListingType.NEWBUILD_PROJECT;
                case "Garage": return ListingType.PARKING;
            }

            return ListingType.MISCELLANEOUS;
        }

        /// <summary>
        /// Get the listing way using the status.
        /// </summary>
        /// <returns>The proper listing way</returns>
        private ListingWay GetListingWay() {
            return Status == "Te Koop" ? ListingWay.SALE : ListingWay.RENT;
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
        private string GetEnergy() {
            return Energy.Split(" ")[0];
        }

        /// <summary>
        /// Create a new list of the right picture objects.
        /// </summary>
        /// <returns>An array of picture objects</returns>
        private ICollection<Picture> GetPictures() {
            ICollection<Picture> pictures = new List<Picture>();

            foreach (string imageUrl in Images.Select(img => img.Url)) {
                pictures.Add(new Picture(imageUrl));
            }

            return pictures;
        }
    }

}
