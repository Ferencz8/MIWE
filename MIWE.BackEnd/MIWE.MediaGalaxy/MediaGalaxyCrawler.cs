using MIWE.Core.Interfaces;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace MIWE.MediaGalaxy
{
    public class MediaGalaxyCrawler : ICrawl
    {

        private bool _disposed = false;
        private ChromeDriver _driver;
        private List<string> proxies;
        private List<MediaGalaxyProduct> MediaGalaxyProducts { get; set; }
        public CancellationToken CancellationToken { get; set; }

        private IEnumerable<string> _productLinks;
        public MediaGalaxyCrawler()
        {
            MediaGalaxyProducts = new List<MediaGalaxyProduct>();
        }


        private void Start()
        {
            ChromeOptions options = new ChromeOptions();

            options.AddArgument("ignore-certificate-errors");
            using (_driver = new ChromeDriver(AppDomain.CurrentDomain.BaseDirectory, options))
            {

                _driver.Navigate().GoToUrl("https://mediagalaxy.ro/telefoane/cpl/");

                Thread.Sleep(2000);

                _productLinks = _driver.FindElementsByXPath("//div[@class='Product-list-center']//h2[@class='Product-nameHeading']/a")
                                       .Where(n => !string.IsNullOrEmpty(n.GetAttribute("href")))
                                       .Select(n => n.GetAttribute("href"))
                                       .ToList();
                
                foreach (var link in _productLinks)
                {
                    try
                    {
                        var product = ParseLink(link);
                        MediaGalaxyProducts.Add(product);
                    }
                    catch (Exception ex)
                    {
                        //TODO:: log exception
                    }
                }
            }
        }

        public bool ScrapeData(CancellationToken? cancellationToken = null)
        {
            try
            {
                if (cancellationToken.HasValue)
                    CancellationToken = cancellationToken.Value;

                Start();
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public MediaGalaxyProduct ParseLink(string link)
        {
            if (CancellationToken != null && CancellationToken.IsCancellationRequested)
            {
                CancellationToken.ThrowIfCancellationRequested();
            }

            _driver.Navigate().GoToUrl(link);

            string name = _driver.FindElementByXPath("//div[@class='Product']//h1")?.Text;

            string price = _driver.FindElementByXPath("//div[@class='Product']//div[@class='Price-current']")?.Text;

            return new MediaGalaxyProduct()
            {
                Name = name,
                Price = price
            };
        }

        public IEnumerable<string> GetProductLinks()
        {
            return _productLinks;
        }

        public IEnumerable<IProductData> GetData()
        {
            return MediaGalaxyProducts;
        }
        public void Dispose() => Dispose(true);

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                if (_driver != null)
                {
                    _driver.Close();
                    _driver.Quit();
                    _driver.Dispose();
                }
            }

            _disposed = true;
        }
    }
}
