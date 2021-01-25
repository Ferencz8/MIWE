using CsvHelper;
using MIWE.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace MIWE.CsvProcessor
{
    public class CsvProcessor : IProcess
    {
        public bool ProcessData(string merchantName, IEnumerable<IProductData> products, Action<MemoryStream, string> saveAction = null)
        {
            try
            {
                string path = Path.Combine(Directory.GetCurrentDirectory(), merchantName);
                using (var memoryStream = new MemoryStream())
                using (var writer = new StreamWriter(memoryStream))
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    csv.WriteRecords(products);
                    writer.Flush();
                    if (saveAction != null)
                    {
                        saveAction.Invoke(memoryStream, ".csv");
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
