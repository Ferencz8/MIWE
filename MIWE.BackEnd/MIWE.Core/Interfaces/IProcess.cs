using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MIWE.Core.Interfaces
{
    public interface IProcess
    {
        bool ProcessData(string merchantName, IEnumerable<IProductData> products, Action<MemoryStream, string> saveAction = null);
    }
}
