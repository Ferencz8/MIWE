using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace MIWE.Core.Interfaces
{
    public interface IProcess
    {
        Task<bool> ProcessData(string merchantName, IEnumerable<IProductData> products, Action<MemoryStream, string> saveAction = null);
    }
}
