using System;
using System.Collections.Generic;
using System.Text;

namespace DnaVastgoed.Models {

    class Property {

        public int Id { get; private set; }
        public DateTime Date { get; private set; }
        public string Slug { get; private set; }
        public string Status { get; private set; }
        public string Type { get; private set; }
        public string Link { get; private set; }
        public string Title { get; private set; }
        public string Content { get; private set; }
        //Type, location, status, label

    }
}
