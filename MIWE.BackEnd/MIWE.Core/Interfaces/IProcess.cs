using System;
using System.Collections.Generic;
using System.Text;

namespace MIWE.Core.Interfaces
{
    public interface IProcess
    {
        bool ProcessData(string merchantName, IEnumerable<IProductData> products);
    }
}
