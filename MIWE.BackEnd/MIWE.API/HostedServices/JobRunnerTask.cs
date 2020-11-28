using Grpc.Net.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MIWE.Core.Interfaces;
using MIWE.Data.Services;
using ProtoBuf.Grpc.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MIWE.API.HostedServices
{
    public class JobRunnerTask : IHostedService, IDisposable
    {
        private Timer _timer;
        private ILogger<JobRunnerTask> _logger;
        private IServiceScopeFactory _serviceScopeFactory;

        public JobRunnerTask(ILogger<JobRunnerTask> logger, IServiceScopeFactory serviceScopeFactory)
        {
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("JobRunnerTask started");
            //while (true)
            {
                try
                {
                    //_timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromHours(1));
                    await DoWork();
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Failed to run updater job with ex{ex}");
                }
                finally
                {
                    Thread.Sleep(TimeSpan.FromMinutes(1));
                }
            }
            _logger.LogInformation("JobRunnerTask finished");
        }

        private async Task DoWork()
        {
            _logger.LogInformation("Timed Background Service is working.");

            var tokenSource = new CancellationTokenSource();
            var token = tokenSource.Token;
            Task jobRunnerTask = Task.Factory.StartNew(async () =>
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    var jobExecuter = scopedServices.GetRequiredService<IJobExecuter>();
                    var instanceService = scopedServices.GetRequiredService<IInstanceService>();
                    
                    while (!token.IsCancellationRequested)
                    {
                        await RunJobs(jobExecuter, instanceService, token);

                        Thread.Sleep(TimeSpan.FromMinutes(1));
                    }

                    if (token.IsCancellationRequested)
                    {
                        token.ThrowIfCancellationRequested();
                    }
                }

            }, token);

            try
            {
                jobRunnerTask.Wait();
            }
            catch (OperationCanceledException operationCanceledEx)
            {
                //log
            }
            catch (Exception genericException)
            {
                //log
            }
            finally
            {
                tokenSource.Dispose();
            }
        }
        private async Task RunJobs(IJobExecuter jobExecuter, IInstanceService instanceService, CancellationToken token)
        {
            var allJobsRan = await jobExecuter.RunAllJobSchedules();
            if (!allJobsRan)
            {
                string ip = instanceService.GetAvailableInstanceIP();
                if (string.IsNullOrEmpty(ip))
                    return;

                var jobs = jobExecuter.GetAllJobs();
                //call instance to run a job

                try
                {
                    GrpcClientFactory.AllowUnencryptedHttp2 = true;
                    var channel = GrpcChannel.ForAddress($"https://localhost:8009");

                    var jobReceiverService = channel.CreateGrpcService<IJobReceiver>();
                    jobReceiverService.ReceiveJob(jobs.ElementAt(0));
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }



        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Timed Background Service is stopping.");
            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
