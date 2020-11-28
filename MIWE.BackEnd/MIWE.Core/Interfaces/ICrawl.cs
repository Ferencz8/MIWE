using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace MIWE.Core.Interfaces
{
    public interface ICrawl : IDisposable
    {
        CancellationToken CancellationToken { get; set; }

        bool ScrapeData(CancellationToken? cancellationToken = null);

        //IProductData ParseLink(string link);

        IEnumerable<string> GetProductLinks();        

        IEnumerable<IProductData> GetData();
    }
}
