using DocumentFormat.OpenXml.Spreadsheet;
using HtmlAgilityPack;
using MIWE.Core.Interfaces;
using Newtonsoft.Json;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MIWE.Imobiliare
{
    public class ImobiliareCrawler : ICrawl
    {

        private string Url = "http://www.imobiliare.ro/vanzare-garsoniere/brasov?pagina={0}";
        private List<ImobiliareProduct> _imobiliareProducts = new List<ImobiliareProduct>();
        private List<string> _productLinks  = new List<string>();
        private HtmlDocument _htmlDocument = new HtmlDocument();
        private List<Proxy> proxies = new List<Proxy>();
        private Proxy ActiveProxyIP;

        public CancellationToken CancellationToken { get; set; }

        public bool ScrapeData(CancellationToken? cancellationToken = null)
        {
            try
            {
                if (cancellationToken.HasValue)
                    CancellationToken = cancellationToken.Value;

                Task startTask = Task.Factory.StartNew(() => Start(), CancellationToken);
                startTask.Wait();
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void GetProxies()
        {

            using (var driver = new ChromeDriver(AppDomain.CurrentDomain.BaseDirectory))
            {
                Thread.Sleep(2000);
                driver.Navigate().GoToUrl("http://free-proxy.cz/en/proxylist/country/RO/all/ping/all");

                Thread.Sleep(2000);
                var proxyIPs = driver.FindElementsByXPath("//table[@id='proxy_list']/tbody/tr/td[1]")
                    .Where(n => !string.IsNullOrEmpty(n.Text))
                    .Select(n => n.Text)
                    .ToList();

                var proxyPorts = driver.FindElementsByXPath("//table[@id='proxy_list']/tbody/tr/td[2]")
                    .Where(n => !string.IsNullOrEmpty(n.Text))
                    .Select(n => n.Text)
                    .ToList();
                for (int i = 0; i < proxyIPs.Count; i++)
                {
                    proxies.Add(new Proxy()
                    {
                        IP = proxyIPs[i],
                        Port = proxyPorts[i]
                    });
                }
            }
        }

        private string GetActiveProxy()
        {
            int minFailCount = proxies.Min(n => n.FailCount);
            ActiveProxyIP = proxies.First(n => n.FailCount <= minFailCount);
            return ActiveProxyIP.Uri;
        }

        private HttpClient GetHttpClientWithProxy()
        {
            WebProxy proxy = new WebProxy
            {
                Address = new Uri(GetActiveProxy()),
            };
            HttpClientHandler clientHandler = new HttpClientHandler()
            {
                AllowAutoRedirect = true,
                AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip,
                Proxy = proxy,
            };
            HttpClient client = new HttpClient(clientHandler);
            return client;
        }

        private HttpClient GetHttpClient()
        {
            HttpClientHandler clientHandler = new HttpClientHandler()
            {
                AllowAutoRedirect = true,
                AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip,
              
            };
            HttpClient client = new HttpClient(clientHandler);
            return client;
        }


        private async Task Start()
        {
            int pageCount = 0;
            int pageTotal = 0;
            bool isPageTotalExtracted = false;
            bool isScrapingFinished = false;
            //GetProxies();

            while (!isScrapingFinished)
            {
                try
                {
                    var client = GetHttpClient();

                    do
                    {
                        HttpResponseMessage result = client.GetAsync(string.Format(Url, pageCount)).Result;
                        var sr = new StreamReader(await result.Content.ReadAsStreamAsync(), Encoding.GetEncoding("iso-8859-1"));
                        
                            var content = sr.ReadToEnd();
                        
                    
                        _htmlDocument.LoadHtml(content);

                        var anouncements = _htmlDocument.DocumentNode.SelectNodes("//div[@itemtype='http://schema.org/Offer']");

                        GetTotalNumberOfPages(ref pageTotal, ref isPageTotalExtracted);

                        foreach (var anouncement in anouncements)
                        {
                            if (CancellationToken != null && CancellationToken.IsCancellationRequested)
                            {
                                CancellationToken.ThrowIfCancellationRequested();
                            }


                            //check in db if the annoucement exists already by multiple ids and date published
                            string anouncementUrl = anouncement.SelectSingleNode("//a[@itemprop='name']").GetAttributeValue("href", string.Empty);
                            if (!string.IsNullOrEmpty(anouncementUrl))
                            {
                                var anouncementContent = await client.GetStringAsync(anouncementUrl);

                                ImobiliareProduct productData = ParseAnouncement(anouncementContent);
                                _imobiliareProducts.Add(productData);
                            }

                            _productLinks.Add(anouncementUrl);
                        }
                    } while (pageCount <= pageTotal);
                    
                    isScrapingFinished = true;
                }
                catch (Exception ex)
                {
                    //if (ex.Message.Contains("403"))
                    //{
                    //    ActiveProxyIP.FailCount++;
                    //}
                }
            }

            string json = JsonConvert.SerializeObject(_imobiliareProducts);
            File.WriteAllText(Path.Combine(Directory.GetCurrentDirectory() , "imobiliare.json"), json);
        }

        private void GetTotalNumberOfPages(ref int pageTotal, ref bool isPageTotalExtracted)
        {
            if (!isPageTotalExtracted)
            {
                isPageTotalExtracted = true;
                pageTotal = _htmlDocument.DocumentNode.SelectSingleNode("//a[@class='butonpaginare'][last()]").GetAttributeValue("data-pagina", 0);
            }
        }

        private ImobiliareProduct ParseAnouncement(string anouncementContent)
        {
            ImobiliareProduct product = new ImobiliareProduct();

            HtmlDocument anouncementDocument = new HtmlDocument();
            anouncementDocument.LoadHtml(anouncementContent);
            HtmlNode htmlNode = anouncementDocument.DocumentNode;
            product.Title = htmlNode.SelectSingleNode("//div[@id='content-detalii']//div[@class='titlu']/h1")?.InnerText;
            product.Area = htmlNode.SelectSingleNode("//div[@id='content-detalii']//div[@class='titlu']/div")?.InnerText;
            product.Price = htmlNode.SelectSingleNode("//div[@id='box-prezentare']//div[@class='pret first blue']")?.InnerText;
            var images = htmlNode.SelectNodes("//li[@class='imagine']//a");
            List<string> imageUrls = new List<string>();
            foreach (var image in images)
            {
                string imagehref = image.GetAttributeValue("href", string.Empty);
                if (!string.IsNullOrEmpty(imagehref))
                {
                    imageUrls.Add(imagehref);
                }
            }
            product.Images = imageUrls;
            product.Details = htmlNode.SelectSingleNode("//div[@id='b_detalii_text']//p")?.InnerText;

            string lastActualized = htmlNode.SelectSingleNode("//div[@id='content-detalii']//span[@class='data-actualizare']")?.InnerText;
            lastActualized = lastActualized.Replace("Actualizat &#238;n", string.Empty);
            ///product.LastPublished = DateTime.Parse(lastActualized.Trim());
            product.LastPublished = lastActualized.Trim();
            var characteristicsLeft = htmlNode.SelectNodes("//ul[@class='lista-tabelara']/li");
            foreach (var characteristic in characteristicsLeft)
            {
                if (characteristic.InnerText.Contains("Nr. camere"))
                {
                    string nrCamere = characteristic.InnerText.Replace("Nr. camere:", string.Empty).Trim();
                    product.Caracteristici.NrCamere = int.Parse(nrCamere);
                }
                else if (characteristic.InnerText.Contains("Suprafaţă utilă:"))
                {
                    string supUtila = characteristic.InnerText.Replace("Suprafaţă utilă:", string.Empty).Replace("mp", "").Trim();
                    product.Caracteristici.SuprafataUtila = int.Parse(supUtila);
                }
            }
            var characteristicsRight = htmlNode.SelectNodes("//ul[@class='lista-tabelara mobile-list']/li");
            product.Specificatii = htmlNode.SelectSingleNode("//div[@id='b_detalii_specificatii']")?.InnerText;
            return product;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IProductData> GetData()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> GetProductLinks()
        {
            throw new NotImplementedException();
        }
    }
}
