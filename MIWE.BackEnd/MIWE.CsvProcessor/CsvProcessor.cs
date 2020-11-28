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
        //TODO::specify as input the path   
        public bool ProcessData(string merchantName, IEnumerable<IProductData> products)
        {
            try
            {
                string path = Path.Combine(Directory.GetCurrentDirectory(), merchantName);
                using (var writer = new StreamWriter($"{path}.csv"))
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    csv.WriteRecords(products);
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
