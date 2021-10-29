using System;
using System.Collections.Generic;

namespace MIWE.Imobiliare
{
    public class ImobiliareProduct
    {
        public ImobiliareProduct()
        {
            Caracteristici = new Caracteristici();
        }
        public string Url { get; set; }

        public string LastPublished { get; set; }

        public string Title { get; set; }

        public string Price { get; set; }

        public string Area { get; set; }

        public string Details { get; set; }

        public Caracteristici Caracteristici { get; set; }

        public string Specificatii { get; set; }

        public string Location { get; set; }

        public string NrTelefon { get; set; }

        public string PublicatDe { get; set; }

        public List<string> Images { get; set; }
    }
}
