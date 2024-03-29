﻿using System;
using System.Collections.Generic;
using System.Text;

namespace MIWE.Core.Interfaces
{
    public interface IProductData
    {

        string Name { get; set; }

        string Price { get; set; }

        string Availability { get; set; }

        string Url { get; set; }

        string ImageUrl { get; set; }
    }
}
