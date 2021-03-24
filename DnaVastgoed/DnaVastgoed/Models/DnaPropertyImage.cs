namespace DnaVastgoed.Models {

    public class DnaPropertyImage {

        public int Id { get; set; }
        public string Url { get; set; }

        public DnaPropertyImage(string url) {
            Url = url;
        }

    }

}
