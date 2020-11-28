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

        public JobExecuter(IInstanceService instanceService, IPluginRunner pluginRunner, IJobRepository jobRepository,
            IJobSessionRepository jobSessionRepository, IJobScheduleRepository jobScheduleRepository, ILogger<JobExecuter> logger)
        {
            _instanceService = instanceService;
            _pluginRunner = pluginRunner;
            _jobRepository = jobRepository;
            _jobSessionRepository = jobSessionRepository;
            _jobScheduleRepository = jobScheduleRepository;
            _logger = logger;
        }

        public IEnumerable<Job> GetAllJobs()
        {
            return _jobRepository.GetAll(n => !n.IsRunning)
                .ToList();
        }

        public async Task<bool> RunAllJobSchedules()
        {
            var jobsToBeRun = _jobScheduleRepository.GetAll(n => !n.IsRunning).ToList();

            int instanceId = _instanceService.GetCurrentInstanceId();

            for (int i = 0; i < jobsToBeRun.Count(); i++)
            {
                var job = jobsToBeRun.ElementAt(i);
                CronExpression expression = new CronExpression(job.Scheduling);
                expression.TimeZone = TimeZoneInfo.Utc;
                DateTimeOffset? nextFireUTCTime = expression.GetNextValidTimeAfter(DateTime.UtcNow);
                if (DateTime.Compare(DateTime.UtcNow, nextFireUTCTime.Value.UtcDateTime) < 0)
                {
                    continue; //skip
                }

                if (_instanceService.IsCPUThresholdReached())
                {
                    return false;
                }
                else
                {
                    //separate tasks TODO
                    //RunJob(instanceId, job.Id, job.PluginPath);
                    await RunScheduledJob(instanceId, job.Id);
                }
            }

            //finished alone all jobs
            return true;
        }

        public bool RunAllJobs()
        {
            var jobsToBeRun = GetAllJobs();

            int instanceId = _instanceService.GetCurrentInstanceId();

            for (int i = 0; i < jobsToBeRun.Count(); i++)
            {
                var job = jobsToBeRun.ElementAt(i);


                if (_instanceService.IsCPUThresholdReached())
                {
                    return false;
                }
                else
                {
                    //separate tasks TODO
                    RunJob(instanceId, job.Id, job.PluginPath);
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
            string dirPath = Path.Combine(Directory.GetCurrentDirectory(), "plugins");
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

        public async Task RunScheduledJob(int instanceId, Guid jobScheduleId)
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
                var mainJobSessionId = await AddJobSession(instanceId, jobSchedule.Id);
                foreach (var jobId in jobIds)
                {
                    var job = await _jobRepository.GetById(jobId);
                    jobs.Add(job);
                }

                //run the plugins
                var mainJob = await _jobRepository.GetById(jobSchedule.MainJob);

                bool result = _pluginRunner.Run(
                    GetCrawlPath(mainJob.PluginPath),
                    GetCrawlPath(jobs.FirstOrDefault().PluginPath),
                    jobs.FirstOrDefault().Name);

                //finish sessions
                await MarkJobSessionsAsEnded(mainJobSessionId, result);

                await MarkScheduledJobAsEnded(mainJob);

                //TODO:: add support for multiple data processors
            }
            catch(Exception ex)
            {
                _logger.LogError($"Failed to run scheduled job with Id: {jobScheduleId}", ex);
            }
        }

        private string GetCrawlPath(string pluginName)
        {
            string dirPath = Path.Combine(Directory.GetCurrentDirectory(), "plugins");
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
