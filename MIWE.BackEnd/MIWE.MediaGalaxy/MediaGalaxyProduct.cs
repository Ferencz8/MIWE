using MIWE.Core.Interfaces;
using System;

namespace MIWE.MediaGalaxy
{
    public class MediaGalaxyProduct : IProductData
    {
        public string Name { get; set; }

        public string Price { get; set; }
        public string Availability { get; set; }
        public string Url { get; set; }
        public string ImageUrl { get; set; }

        public MediaGalaxyProduct()
        {

        }
    }
}
