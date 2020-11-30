using MIWE.Core;
using MIWE.Core.Interfaces;
using MIWE.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using MIWE.Data.Entities;
using MIWE.Data.Services;
using MIWE.Data.Services.Interfaces;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Extensions.Logging;
using Quartz;
using MIWE.Core.Models;
using Microsoft.Extensions.Configuration;
using MIWE.Common;

namespace MIWE.Core
{
    public class JobExecuter : IJobExecuter
    {

        private IInstanceService _instanceService;
        private IPluginRunner _pluginRunner;
        private IJobRepository _jobRepository;
        private IJobSessionRepository _jobSessionRepository;
        private IJobScheduleRepository _jobScheduleRepository;
        private ILogger<JobExecuter> _logger;
        private IConfiguration _configuration;
        private IWorkloadAnalyzer _workloadAnalyzer;

        public JobExecuter(IInstanceService instanceService, IPluginRunner pluginRunner, IJobRepository jobRepository,
            IJobSessionRepository jobSessionRepository, IJobScheduleRepository jobScheduleRepository, ILogger<JobExecuter> logger,
            IConfiguration configuration, IWorkloadAnalyzer workloadAnalyzer)
        {
            _instanceService = instanceService;
            _pluginRunner = pluginRunner;
            _jobRepository = jobRepository;
            _jobSessionRepository = jobSessionRepository;
            _jobScheduleRepository = jobScheduleRepository;
            _logger = logger;
            _configuration = configuration;
            _workloadAnalyzer = workloadAnalyzer;
        }

        public IEnumerable<JobSchedule> GetAllJobsScheduled()
        {
            var jobsToBeRun = _jobScheduleRepository.GetJobsScheduledToRun();

            var awaitingJobs = _workloadAnalyzer.ScanJobsAwaitingToBeRun(jobsToBeRun);
            return _jobScheduleRepository.GetAll(n => awaitingJobs.Contains(n.Id))
                .ToList();
        }

        public async Task<bool> RunAllJobSchedules(CancellationToken? cancellationToken)
        {
            int instanceId = _instanceService.GetCurrentInstanceId();
            var awaitingJobs = GetAllJobsScheduled();

            foreach (JobSchedule jobScheduled in awaitingJobs)
            {
                if (_instanceService.IsCPUThresholdReached())
                {
                    return false;
                }
                else
                {
                    //separate tasks TODO
                    await RunScheduledJob(instanceId, jobScheduled.Id, cancellationToken);
                }
            }

            //finished alone all jobs
            return true;
        }

        public async Task RunJob(int instanceId, Guid jobId, string crawlPath, CancellationToken? cancellationToken = null)
        {
            //register in db
            var addedEntity = await _jobSessionRepository.Create(new JobSession()
            {
                DateStart = DateTime.UtcNow,
                InstanceId = instanceId,
                EntityId = jobId
            });

            Guid addedSessionId = addedEntity.Id;

            var job = await _jobRepository.GetById(jobId);
            job.IsRunning = true;
            await _jobRepository.Update(job);

            //string dirPath = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName, "plugins");
            //pluginRunner.Run(Path.Combine(dirPath, "EmagCrawler\\netcoreapp3.1\\DemoPlugin.EmagCrawler.dll"));
            string dirPath = Path.Combine(Directory.GetCurrentDirectory(), _configuration.GetSection(Constants.PluginFolder).Value);
            bool result = _pluginRunner.Run(Path.Combine(dirPath, crawlPath), cancellationToken);

            //update in db completed
            var ranJobSession = await _jobSessionRepository.GetById(addedSessionId);
            ranJobSession.DateEnd = DateTime.UtcNow;
            ranJobSession.IsSuccess = result;
            await _jobSessionRepository.Update(ranJobSession);

            var ranJob = await _jobRepository.GetById(jobId);
            ranJob.IsRunning = false;
            await _jobRepository.Update(ranJob);
        }

        public async Task RunScheduledJob(int instanceId, Guid jobScheduleId, CancellationToken? cancellationToken = null)
        {
            try
            {
                JobSchedule jobSchedule;
                List<Job> jobs = new List<Job>();
                List<Guid> jobSessionsIds = new List<Guid>();

                jobSchedule = await _jobScheduleRepository.GetById(jobScheduleId);

                List<Guid> jobIds = jobSchedule.NextJobs.Split(',')
                    .Where(n => !string.IsNullOrEmpty(n))
                    .Select(n => Guid.Parse(n))
                    .ToList();//,-> one after another ;-> paralel

                //start sessions
                var jobSessionId = await AddJobSession(instanceId, jobSchedule.Id);
                foreach (var jobId in jobIds)
                {
                    var job = await _jobRepository.GetById(jobId);
                    jobs.Add(job);
                }

                //run the plugins
                var mainJob = await _jobRepository.GetById(jobSchedule.MainJob);

                bool result = _pluginRunner.Run(new PluginRunningParameters()
                {
                    MerchantName = jobs.FirstOrDefault().Name,
                    CrawlerPluginPath = GetCrawlPath(mainJob.PluginPath),
                    ProcessorPluginPath = GetCrawlPath(jobs.FirstOrDefault().PluginPath),
                    ProcessorSaveAction = GetSaveProcessedActionData(jobSessionId)
                }, cancellationToken);

                //finish sessions
                await MarkJobSessionsAsEnded(jobSessionId, result);

                await MarkScheduledJobAsEnded(mainJob);

                //TODO:: add support for multiple data processors
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to run scheduled job with Id: {jobScheduleId}", ex);
            }
        }

        private Action<MemoryStream, string> GetSaveProcessedActionData(Guid jobSessionId)
        {
            return async (memoryStream, extension) =>
            {
                string path = Path.Combine(Directory.GetCurrentDirectory(), _configuration.GetSection(Constants.ProcessedDataFolder).Value);
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                string filePath = Path.Combine(path, $"{jobSessionId}{extension}");
                using (var stream = new FileStream(filePath, FileMode.OpenOrCreate))
                {
                    memoryStream.CopyTo(stream);
                }

                var jobSessionEntity = await _jobSessionRepository.GetById(jobSessionId);
                jobSessionEntity.ResultPath = filePath;
                jobSessionEntity.ResultContentType = "text/csv";
                await _jobSessionRepository.Update(jobSessionEntity);
            };
        }

        private string GetCrawlPath(string pluginName)
        {
            string dirPath = Path.Combine(Directory.GetCurrentDirectory(), _configuration.GetSection(Constants.PluginFolder).Value);
            string pluginPath = Path.Combine(dirPath, pluginName);
            return pluginPath;
        }

        private async Task MarkScheduledJobAsEnded(Job mainJob)
        {
            var ranJob = await _jobRepository.GetById(mainJob.Id);
            ranJob.IsRunning = false;
            await _jobRepository.Update(ranJob);
        }

        private async Task MarkJobSessionsAsEnded(Guid mainJobSessionId, bool result)
        {
            var ranJobSession = await _jobSessionRepository.GetById(mainJobSessionId);
            ranJobSession.DateEnd = DateTime.UtcNow;
            ranJobSession.IsSuccess = result;
            await _jobSessionRepository.Update(ranJobSession);
        }

        private async Task<Guid> AddJobSession(int instanceId, Guid entityId)
        {

            var job = await _jobSessionRepository.Create(new JobSession()
            {
                DateStart = DateTime.UtcNow,
                InstanceId = instanceId,
                EntityId = entityId
            });
            var jobSchedule = await _jobScheduleRepository.GetById(entityId);
            jobSchedule.IsRunning = true;
            await _jobScheduleRepository.Update(jobSchedule);

            return job.Id;
        }
    }
}
