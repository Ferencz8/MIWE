using MIWE.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace MIWE.MercadoLivre
{
    public class MercadoLivreProduct : IProductData
    {
        public string Name { get; set; }

        public string Price { get; set; }
    }
}
