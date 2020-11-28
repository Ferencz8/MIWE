using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace MIWE.API
{
    
    //https://anthonygiretti.com/2020/03/29/grpc-asp-net-core-3-1-whats-grpc-web-how-to-create-a-grpc-web-service/
    //https://stackoverflow.com/questions/56890644/grpc-and-mvc-in-same-asp-net-core-3-0-application

    public class Program
    {
        private static int OpenedPort;

        public static void Main(string[] args)
        {
            SetOpenedPort();

            CreateWebHostBuilder(args).Build().Run();
        }

        private static void SetOpenedPort()
        {
            var config = new ConfigurationBuilder()
                            .AddJsonFile("appsettings.json", optional: false)
                            .Build();
            if (!Int32.TryParse(config.GetSection("OpenedPort").Value, out OpenedPort))
            {
                OpenedPort = 8008;
            }
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
             .ConfigureKestrel(options =>
             {

                 options.Limits.MinRequestBodyDataRate = null;

                 options.ListenLocalhost(OpenedPort, listenOptions =>
                 {
                     listenOptions.Protocols = HttpProtocols.Http1AndHttp2;
                     listenOptions.UseHttps(adapterOptions =>
                     {
                         adapterOptions.ClientCertificateMode = ClientCertificateMode.NoCertificate;
                         adapterOptions.ServerCertificate = null;
                     });
                 });
             })
             .UseStartup<Startup>();
    }
}
