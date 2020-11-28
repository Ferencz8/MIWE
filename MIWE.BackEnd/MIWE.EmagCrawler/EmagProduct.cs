using MIWE.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace MIWE.EmagCrawler
{
    public class EmagProduct : IProductData
    {
        public string Name { get; set; }

        public string Price { get; set; }

        public EmagProduct()
        {

        }
    }
}
