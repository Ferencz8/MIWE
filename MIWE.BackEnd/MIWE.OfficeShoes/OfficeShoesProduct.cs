using MIWE.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace MIWE.OfficeShoes
{
    public class OfficeShoesProduct : IProductData
    {
        public string Name { get; set; }

        public string Price { get; set; }
    }
}
