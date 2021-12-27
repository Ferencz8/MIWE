using MIWE.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace MIWE.Emag
{
    public class EmagProduct : IProductData
    {
        public string Name { get; set; }

        public string Price { get; set; }
        public string Availability { get; set; }
        public string Url { get; set; }
        public string ImageUrl { get; set; }

        public EmagProduct()
        {

        }
    }
}
