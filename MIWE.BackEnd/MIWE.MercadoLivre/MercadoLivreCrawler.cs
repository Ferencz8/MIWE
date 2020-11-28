using MIWE.Core;
using MIWE.Core.Interfaces;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Threading;

namespace MIWE.MercadoLivre
{
    public class MercadoLivreCrawler : ICrawl
    {
        private List<MercadoLivreProduct> _mercadoLivreProducts; 
        private List<string> _productLinks;
        private HtmlDocument _htmlDocument;

        public CancellationToken CancellationToken { get; set; }

        public MercadoLivreCrawler()
        {
            _mercadoLivreProducts = new List<MercadoLivreProduct>();
            _productLinks = new List<string>();
        }

        public IEnumerable<string> GetProductLinks()
        {
            return _productLinks;
        }

        public ResultObj ImportData()
        {
            throw new NotImplementedException();
        }

        public bool ScrapeData(CancellationToken? cancellationToken = null)
        {
            try
            {
                if (cancellationToken.HasValue)
                    CancellationToken = cancellationToken.Value;
                //TODO:: link the 2 cancellation tokens
                Task startTask = Task.Factory.StartNew(()=> Start());
                startTask.Wait();
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private async Task Start()
        {
            int pageCount = 0;
            using (var client = new HttpClient())
            {
                do
                {
                    var content = await client.GetStringAsync($"https://telefonia.mercadolivre.com.br/telefone_Desde_{pageCount}");
                    _htmlDocument.LoadHtml(content);

                    var elements = _htmlDocument.DocumentNode.SelectNodes("//div[@class = 'ui-search-result__wrapper']");
                    foreach (var element in elements)
                    {
                        var link = element.SelectSingleNode("//a[@class='ui-search-item__group__element ui-search-link']")?.GetAttributeValue("href", string.Empty);
                        var name = element.SelectSingleNode("//h2[@class='ui-search-item__title']")?.InnerText;
                        var price = element.SelectSingleNode("//div[contains(@class,'ui-search-item__group__element')]//span[@class='price-tag ui-search-price__part']")?.InnerText;
                        _mercadoLivreProducts.Add(new MercadoLivreProduct()
                        {
                            Name = name,
                            Price = price
                        });
                        _productLinks.Add(link);
                    }
                    pageCount += 50;
                } while (pageCount < 1000);
            }
        }

        public IEnumerable<IProductData> GetData()
        {
            return _mercadoLivreProducts;
        }

        public void Dispose() { }
    }
}
