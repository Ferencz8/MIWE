using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MIWE.Data.Entities
{
    public class Product
    {

        [Key]
        public Guid Id { get; set; }

        public string SKU { get; set; }

        public string Name { get; set; }

        public string Price { get; set; }

        public string Availability { get; set; }

        public string Url { get; set; }

        public string ImageUrl { get; set; }

        public string MerchantName { get; set; }

        public DateTime DateAdded { get; set; }
    }
}
