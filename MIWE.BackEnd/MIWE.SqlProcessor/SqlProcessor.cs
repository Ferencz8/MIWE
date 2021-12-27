using MIWE.Core.Interfaces;
using MIWE.Data.Services;
using MIWE.Data.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace MIWE.SqlProcessor
{
    public class SqlProcessor : IProcess
    {
        private readonly IProductRepository _productRepository;
        private readonly IAzureBlobRepository _azureBlobRepository;

        public SqlProcessor(IProductRepository productRepository, IAzureBlobRepository azureBlobRepository)
        {
            _productRepository = productRepository;
            _azureBlobRepository = azureBlobRepository;
        }

        public async Task<bool> ProcessData(string merchantName, IEnumerable<IProductData> products, Action<MemoryStream, string> saveAction = null)
        {
            try
            {

                using (HttpClient client = new HttpClient())
                {
                    foreach (var product in products)
                    {
                        Stream imageStream = await client.GetStreamAsync(product.ImageUrl);

                        string imageUrl = await _azureBlobRepository.UploadImageAsync(product.Name, imageStream, true);

                        await _productRepository.Create(new Data.Entities.Product()
                        {
                            Availability = product.Availability,
                            MerchantName = merchantName,
                            Name = product.Name,
                            Price = product.Price,
                            Url = product.Url,
                            ImageUrl = imageUrl,
                            DateAdded = DateTime.UtcNow
                        });
                    }
                }

                return true;
            }
            catch (Exception exception)
            {
                //log
                return false;
            }
        }
    }
}
