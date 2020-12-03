﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MIWE.Data.Services;
using MIWE.Data.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace MIWE.API.HostedServices
{
    public class SelfRecoveryBackgroundTask : BackgroundService
    {
        private ILogger<SelfRecoveryBackgroundTask> _logger;
        private IServiceScopeFactory _serviceScopeFactory;
        private JobRunnerBackgroundTask _jobRunnerBackgroundTask;

        public SelfRecoveryBackgroundTask(ILogger<SelfRecoveryBackgroundTask> logger, IServiceScopeFactory serviceScopeFactory,
            JobRunnerBackgroundTask jobRunnerBackgroundTask)
        {
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
            _jobRunnerBackgroundTask = jobRunnerBackgroundTask;
        }

        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Delay(10000);
            {
                try
                {
                    _logger.LogInformation("SelfRecoveryBackgroundTask started");
                    await DoWork(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Failed to run self recovery task with ex: {ex}");
                }
                finally
                {
                    //Thread.Sleep(TimeSpan.FromMinutes(1));
                    _logger.LogInformation("SelfRecoveryBackgroundTask finished");
                }
            }
        }

        private async Task DoWork(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Timed Recovery Background Service started.");

            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var instanceService = scopedServices.GetRequiredService<IInstanceService>();
                var instanceRepo = scopedServices.GetRequiredService<IInstanceRepository>();
                int currentId = instanceService.GetCurrentInstanceId();
                var masterInstance = instanceService.GetMasterInstance();
                while (!cancellationToken.IsCancellationRequested)
                {
                    bool isUP = await PoolMasterAvailability(masterInstance);
                    if (!isUP) //masteer is down
                    {
                        _logger.LogInformation("Changing Master");

                        await instanceRepo.ChangeMaster(currentId);

                        await _jobRunnerBackgroundTask.StartAsync(new CancellationToken());
                        
                        break;
                    }
                    else
                    {
                        var result = await instanceRepo.CheckOrChangeMaster(currentId);
                        if(result >= 1) // master is down and was changed already
                        {
                            _logger.LogInformation("Master Changed");

                            await _jobRunnerBackgroundTask.StartAsync(new CancellationToken());
                        }
                    }

                    Thread.Sleep(TimeSpan.FromMinutes(1));
                }

                if (cancellationToken.IsCancellationRequested)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                }
            }
        }

        private static async Task<bool> PoolMasterAvailability(Data.Instance masterInstance)
        {
            int failed = 0;
            do
            {
                try
                {
                    using (HttpClient client = new HttpClient())
                    {
                        HttpResponseMessage response = await client.GetAsync($"{masterInstance.IP}/hc");
                        response.EnsureSuccessStatusCode();
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    failed++;
                }
            }
            while (failed < 3);
            return false;
        }
    }
}