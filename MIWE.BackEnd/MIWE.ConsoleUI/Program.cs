//using MIWE.Core;
//using MIWE.Core.Interfaces;
//using MIWE.Data;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Logging;
//using System;
//using System.Linq;
//using System.Threading;
//using System.Threading.Tasks;
//using MIWE.Data.Services;
//using MIWE.Data.Entities;

//namespace MIWE
//{
//    class Program
//    {

//        private static ServiceProvider _serviceProvider;
//        static void Main(string[] args)
//        {
//            Console.WriteLine("Hello!");
//            System.AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionHandler;
//            ConfgureServices();

//            SaveFirstJob();
//            var instanceService = _serviceProvider.GetService<IInstanceService>();
//            DecideMaster(instanceService);

//            var tokenSource = new CancellationTokenSource();
//            var token = tokenSource.Token;
//            Task jobRunnerTask = Task.Factory.StartNew(() =>
//            {
//                while (!token.IsCancellationRequested)
//                {
//                    RunJobs(instanceService, token);

//                    Thread.Sleep(TimeSpan.FromSeconds(10));
//                }

//                if (token.IsCancellationRequested)
//                {
//                    //Console.WriteLine("Task {0} was cancelled before it got started.",
//                    //              taskNum);
//                    token.ThrowIfCancellationRequested();
//                }
//            }, token);


//            try
//            {
//                jobRunnerTask.Wait();
//            }
//            catch (OperationCanceledException operationCanceledEx)
//            {
//                //log
//            }
//            catch (Exception genericException)
//            {
//                //log
//            }
//            finally
//            {
//                tokenSource.Dispose();
//            }

//            //EmagCrawler crawler = new EmagCrawler();
//            //var links = crawler.GetProductLinks();

//            //var data = crawler.GetData();
//        }

//        private static void UnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs e)
//        {
//            Console.WriteLine(e.ExceptionObject.ToString());
//            Console.WriteLine("Press Enter to continue");
//            var instanceService = _serviceProvider.GetService<IInstanceService>();
//            instanceService.MarkInstanceDown(true);
//            Console.ReadLine();
//            Environment.Exit(1);
//        }

//        private static void DecideMaster(IInstanceService instanceService)
//        {
//            bool isMasterRegistered = instanceService.IsMasterRegistered();
//            if (isMasterRegistered)
//            {
//                Console.WriteLine("This instance will run as a Slave");
//                instanceService.RegisterInstance(false);
//            }
//            else
//            {
//                Console.WriteLine("This instance will run as a Master");
//                instanceService.RegisterInstance(true);
//            }
//        }

//        private static void ConfgureServices()
//        {
//            //setup our DI
//            _serviceProvider = new ServiceCollection()
//                .AddLogging()
//                .AddSingleton<IJobExecuter, JobExecuter>()
//                .AddSingleton<IJobReceiver, JobReceiver>()
//                .AddSingleton<IInstanceService, InstanceService>()
//                .BuildServiceProvider();

//            ////configure console logging
//            //serviceProvider
//            //    .GetService<ILoggerFactory>()
//            //    .AddConsole(LogLevel.Debug);

//            //var logger = serviceProvider.GetService<ILoggerFactory>()
//            //    .CreateLogger<Program>();
//            //logger.LogDebug("Starting application");

//            //logger.LogDebug("All done!");

//        }

//        private async static Task RunJobs(IInstanceService instanceService, CancellationToken token)
//        {
            
//            IJobExecuter jobExecuter = _serviceProvider.GetService<IJobExecuter>();
//            instanceService.IsCPUThresholdReached();
//            //jobExecuter.RunJob("");
//            var allJobsRan = await jobExecuter.RunAllJobSchedules(token);
//            if (!allJobsRan)
//            {
//                string ip = instanceService.GetAvailableInstanceIP();
//                var jobs = jobExecuter.GetAllJobsScheduled();
//                //call instance to run a job
//            }
//        }

//        private static void SaveFirstJob()
//        {
//            using (var context = new WorkerContext())
//            {
//                var existingJob = context.Jobs.FirstOrDefault(n => n.PluginPath == "EmagCrawler\\netcoreapp3.1\\DemoPlugin.EmagCrawler.dll");
//                if (existingJob != null)
//                    return;
//                var job = new Job()
//                {
//                    DateAdded = DateTime.UtcNow,
//                    PluginPath = "EmagCrawler\\netcoreapp3.1\\DemoPlugin.EmagCrawler.dll",
//                    OSType = OSType.Any,
//                    Name = "Emag Crawler",
//                    IsActive = true
//                };

//                context.Jobs.Add(job);
//                context.SaveChanges();
//            }
//        }
//    }
//}
