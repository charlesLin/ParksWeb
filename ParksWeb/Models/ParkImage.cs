using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ParksWeb.Models
{
    public class ParkImage
    {
        public int ParkId { get; set; }

        public string Filename { get; set; }

        public string Description { get; set; }

        public string ImageUri { get; set; }
        public string ContentType { get; set; }
    }
}