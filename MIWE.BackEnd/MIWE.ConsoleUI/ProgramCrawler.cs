using System;
using System.Collections.Generic;
using System.Text;
using MIWE.Imobiliare;
using MIWE.OfficeShoes;

namespace MIWE.ConsoleUI
{
    class ProgramCrawler
    {
        static void Main(string[] args)
        {
            //    var crawler = new OfficeShoesCrawler();
            //    crawler.ScrapeData();

            var crawler = new Emag.EmagCrawler();
            crawler.ScrapeData();

            //var crawler = new ImobiliareCrawler();
            //crawler.ScrapeData();

            Console.ReadKey();
        }
    }
}
