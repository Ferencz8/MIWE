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
        private List<string> proxies;
        private List<EmagProduct> EmagProducts { get; set; }
        public CancellationToken CancellationToken { get; set; }

        private IEnumerable<string> _productLinks;
        public EmagCrawler()
        {
            EmagProducts = new List<EmagProduct>();
        }

        private void SearchForProxies()
        {
            //using (_driver = new ChromeDriver(AppDomain.CurrentDomain.BaseDirectory))
            //{
            //    _driver.Navigate().GoToUrl("http://free-proxy.cz/en/proxylist/country/GB/all/ping/all");

            //    Thread.Sleep(2000);

            //    var proxyIPs = _driver.FindElementsByXPath("//table[@id='proxy_list']/tbody/tr/td[1]")
            //        .Where(n => !string.IsNullOrEmpty(n.Text))
            //        .Select(n => n.Text)
            //        .ToList();

            //    var proxyPorts = _driver.FindElementsByXPath("//table[@id='proxy_list']/tbody/tr/td[2]")
            //        .Where(n => !string.IsNullOrEmpty(n.Text))
            //        .Select(n => n.Text)
            //        .ToList();
            //    proxies = new List<string>();
            //    for (int i = 0; i < proxyIPs.Count; i++)
            //    {
            //        proxies.Add($"{proxyIPs[i]}:{proxyPorts[i]}");
            //    }
            //}
            using (_driver = new ChromeDriver(AppDomain.CurrentDomain.BaseDirectory))
            {
                _driver.Navigate().GoToUrl("http://www.freeproxylists.net/");

                Thread.Sleep(2000);

                var proxyIPs = _driver.FindElementsByXPath("//table[@class='DataGrid']//tr[@class='Odd']//a | //table[@class='DataGrid']//tr[@class='Even']//a")
                    .Where(n => !string.IsNullOrEmpty(n.Text))
                    .Select(n => n.Text)
                    .ToList();

                var proxyPorts = _driver.FindElementsByXPath("//table[@class='DataGrid']//tr[@class='Odd']/td[2] | //table[@class='DataGrid']//tr[@class='Even']/td[2]")
                    .Where(n => !string.IsNullOrEmpty(n.Text))
                    .Select(n => n.Text)
                    .ToList();
                proxies = new List<string>();
                for (int i = 0; i < proxyIPs.Count; i++)
                {
                    proxies.Add($"{proxyIPs[i]}:{proxyPorts[i]}");
                }
            }

        
            
        }

        private void Start()
        {
            SearchForProxies();
            bool retryProxy = true;
            for(int i=0;i<proxies.Count && retryProxy; i++)
            {
                try
                {
                    ChromeOptions options = new ChromeOptions();
                    Proxy proxy = new Proxy();
                    proxy.SslProxy = proxies[i];
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
                        if (_productLinks.Count() == 0)
                            continue;
                        foreach (var link in _productLinks)
                        {
                            try
                            {
                                var product = ParseLink(link);
                                EmagProducts.Add(product);
                            }
                            catch (Exception ex)
                            {
                                //TODO:: log exception
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("ERR_PROXY_CONNECTION_FAILED") || ex.Message.Contains("ERR_CONNECTION_RESET"))
                        retryProxy = true;
                    else
                        retryProxy = false;
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
