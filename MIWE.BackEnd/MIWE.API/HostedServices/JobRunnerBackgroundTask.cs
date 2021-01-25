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
    public class JobRunnerBackgroundTask : BackgroundService
    {
        private ILogger<JobRunnerBackgroundTask> _logger;
        private IServiceScopeFactory _serviceScopeFactory;

        public JobRunnerBackgroundTask(ILogger<JobRunnerBackgroundTask> logger, IServiceScopeFactory serviceScopeFactory)
        {
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
        }

        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            //while (true)
            await Task.Delay(10000);
            {
                try
                {
                    _logger.LogInformation("JobRunnerBackgroundTask started");
                    await DoWork(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Failed to run updater job with ex: {ex}");
                }
                finally
                {
                    //Thread.Sleep(TimeSpan.FromMinutes(1));
                    _logger.LogInformation("JobRunnerBackgroundTask finished");
                }
            }
        }

        private async Task DoWork(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Timed Background Service started.");

            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var jobExecuter = scopedServices.GetRequiredService<IJobExecuter>();
                var instanceService = scopedServices.GetRequiredService<IInstanceService>();

                while (!cancellationToken.IsCancellationRequested)
                {
                    await RunJobs(jobExecuter, instanceService, cancellationToken);

                    Thread.Sleep(TimeSpan.FromMinutes(1));
                }

                if (cancellationToken.IsCancellationRequested)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                }
            }
        }

        private async Task RunJobs(IJobExecuter jobExecuter, IInstanceService instanceService, CancellationToken token)
        {
            var allJobsRan = await jobExecuter.RunAllJobSchedules(token);
            if (!allJobsRan)
            {
                var jobSchedules = jobExecuter.GetAllJobsScheduled();

                //call instance to run a job
                for (int i = 0; i < jobSchedules.Count(); i++)
                {
                    string ip = string.Empty;
                    try
                    {
                        ip = instanceService.GetAvailableInstanceIP();
                        if (string.IsNullOrEmpty(ip))
                            return;

                        GrpcClientFactory.AllowUnencryptedHttp2 = true;
                        //var channel = GrpcChannel.ForAddress($"https://localhost:8009");//ip
                        var channel = GrpcChannel.ForAddress($"{ip}");

                        var jobReceiverService = channel.CreateGrpcService<IJobReceiver>();
                        jobReceiverService.ReceiveJobSchedule(jobSchedules.ElementAt(i));

                        Thread.Sleep(TimeSpan.FromSeconds(10));
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"Failed to run jobs for ip {ip} and job schedule id: {jobSchedules.ElementAt(i)?.Id}", ex);
                    }
                }
            }
        }
    }
}
