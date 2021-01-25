using MIWE.Core.Interfaces;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace MIWE.OfficeShoes
{
    public class OfficeShoesCrawler : ICrawl
    {
        
        private bool _disposed = false;
        private ChromeDriver _driver;
        private List<string> proxies;
        private List<OfficeShoesProduct> OfficeShoesProducts { get; set; }
        public CancellationToken CancellationToken { get; set; }

        private IEnumerable<string> _productLinks;
        public OfficeShoesCrawler()
        {
            OfficeShoesProducts = new List<OfficeShoesProduct>();
        }


        private void Start()
        {
            ChromeOptions options = new ChromeOptions();

            options.AddArgument("ignore-certificate-errors");
            using (_driver = new ChromeDriver(AppDomain.CurrentDomain.BaseDirectory, options))
            {

                _driver.Navigate().GoToUrl("https://www.officeshoes.ro/pantofi-Barbati/3/48/order_asc");

                Thread.Sleep(2000);

                _productLinks = _driver.FindElementsByXPath("//div[@class='img-articles-main-wrapper']/a[2]")
                                       .Where(n => !string.IsNullOrEmpty(n.GetAttribute("href")))
                                       .Select(n => n.GetAttribute("href"))
                                       .ToList();

                foreach (var link in _productLinks)
                {
                    try
                    {
                        var product = ParseLink(link);
                        OfficeShoesProducts.Add(product);
                        Thread.Sleep(TimeSpan.FromSeconds(5));
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

        public OfficeShoesProduct ParseLink(string link)
        {
            if (CancellationToken != null && CancellationToken.IsCancellationRequested)
            {
                CancellationToken.ThrowIfCancellationRequested();
            }

            _driver.Navigate().GoToUrl(link);

            string name = _driver.FindElementByXPath("//h1[@class='product_show_title']/span[@itemprop='name']")?.Text;

            string price = _driver.FindElementByXPath("//div[@class='product-price']//span[@class='price']")?.Text;

            return new OfficeShoesProduct()
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
            return OfficeShoesProducts;
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
