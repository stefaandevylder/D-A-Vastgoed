namespace DnaVastgoed.Models {

    public class PropertyImage {

        public int Id { get; set; }
        public string Url { get; set; }

        public PropertyImage(string url) {
            Url = url;
        }

    }

}
