using MIWE.Core;
using MIWE.Core.Interfaces;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace MIWE.EmagCrawler
{
    public class EmagCrawler : ICrawl
    {

        private bool _disposed = false;
        private ChromeDriver _driver;
        private List<EmagProduct> EmagProducts { get; set; }
        public CancellationToken CancellationToken { get; set; }

        private IEnumerable<string> _productLinks;
        public EmagCrawler()
        {
            EmagProducts = new List<EmagProduct>();
        }

        private void Start()
        {
            ChromeOptions options = new ChromeOptions();
            Proxy proxy= new Proxy();
            proxy.SslProxy = "188.119.150.33:3128";
            proxy.Kind = ProxyKind.Manual;
            options.Proxy = proxy;
            options.AddArgument("ignore-certificate-errors");
            using (_driver = new ChromeDriver(AppDomain.CurrentDomain.BaseDirectory, options))
            {

                _driver.Navigate().GoToUrl("https://www.emag.ro/televizoare/c?ref=hp_menu_quick-nav_190_1&type=category");

                Thread.Sleep(2000);

                _productLinks = _driver.FindElementsByXPath("//div[@id='card_grid']//a[contains(@class,'product-title')]")
                                       .Where(n => !string.IsNullOrEmpty(n.GetAttribute("href")))
                                       .Select(n => n.GetAttribute("href"))
                                       .ToList();

                foreach (var link in _productLinks)
                {
                    try
                    {
                        var product = ParseLink(link);
                        EmagProducts.Add(product);
                    }
                    catch(Exception ex)
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

        public EmagProduct ParseLink(string link)
        {
            if (CancellationToken != null && CancellationToken.IsCancellationRequested)
            {
                CancellationToken.ThrowIfCancellationRequested();
            }

            _driver.Navigate().GoToUrl(link);

            string name = _driver.FindElementByXPath("//h1[@class='page-title']")?.Text;

            string price = _driver.FindElementByXPath("//form[@class='main-product-form']//p[contains(@class,'product-new-price')]")?.Text;

            return new EmagProduct()
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
            return EmagProducts;
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
                if(_driver != null)
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
