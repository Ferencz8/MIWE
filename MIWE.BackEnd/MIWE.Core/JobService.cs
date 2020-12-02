using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MIWE.Core.Interfaces;
using MIWE.Core.Models;
using MIWE.Data;
using MIWE.Data.Dtos;
using MIWE.Data.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MIWE.Core
{
    //TODO:: move db interaction to data dll
    public class JobService : IJobService
    {
        private IJobExecuter _jobExecuter;
        private TaskSettings _taskSettings;
        private ILogger _logger;
        private IJobRepository _jobRepository;
        private IInstanceRepository _intanceRepository;
        private IJobSessionRepository _jobSessionRepository;
        private IServiceScopeFactory _serviceScopeFactory;
        public JobService(IJobExecuter jobExecuter, TaskSettings taskSettings, ILogger<JobService> logger,
            IJobRepository jobRepository, IInstanceRepository instanceRepository, IJobSessionRepository jobSessionRepository,
            IServiceScopeFactory serviceScopeFactory)
        {
            _jobExecuter = jobExecuter;
            _taskSettings = taskSettings;
            _logger = logger;
            _jobRepository = jobRepository;
            _intanceRepository = instanceRepository;
            _jobSessionRepository = jobSessionRepository;
            _serviceScopeFactory = serviceScopeFactory;
        }

        public async Task<Guid> Add(Job job)
        {
            var addedEntity = await _jobRepository.Create(job);
            return addedEntity.Id;
        }

        public IEnumerable<Job> Get()
        {
            return _jobRepository.GetAll().ToList();
        }

        public async Task<Job> GetById(Guid jobId)
        {
            return await _jobRepository.GetById(jobId);
        }

        public IEnumerable<Job> GetByIds(IEnumerable<Guid> ids)
        {
            return _jobRepository.GetAll(n => ids.Contains(n.Id));
        }

        public async Task MarkStopped(int instanceId)
        {
            await _jobRepository.MarkStopped(instanceId);
        }

        public void StartJob(Guid jobId)
        {
            if (_taskSettings.Tasks.ContainsKey(jobId))
            {
                _logger.LogWarning($"Multiple attempts to start Job with Id: {jobId}");
                return;
            }

            int? instanceId = _intanceRepository.GetAll(n => !n.IsDown && n.IsAvailable).FirstOrDefault()?.Id;
            if (!instanceId.HasValue)
                throw new Exception("No available instances.");

            var job = _jobRepository.GetAll(n => !n.IsRunning && n.Id == jobId).FirstOrDefault();
            if (job == null)
            {
                _logger.LogWarning($"No available job found for Id: {job}");
                return;
            }
            string crawlPath = job.PluginPath;

            var tokenSource = new CancellationTokenSource();
            var token = tokenSource.Token;
            Task.Factory.StartNew(async () =>
            {
                try
                {
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        var jobExecuterService = scope.ServiceProvider.GetRequiredService<IJobExecuter>();
                        await jobExecuterService.RunJob(instanceId.Value, jobId, crawlPath, token);

                        tokenSource.Dispose();

                        _taskSettings.Tasks.TryRemove(jobId, out tokenSource);
                    }
                }
                catch(Exception ex)
                {
                    _logger.LogError($"Failed to run job with id: {jobId}", ex);
                }

            }, token);

            _taskSettings.Tasks.TryAdd(jobId, tokenSource);
        }

        public void StopJob(Guid scheduledJobId)
        {
            try
            {
                CancellationTokenSource cancellationTokenSource;
                if (_taskSettings.Tasks.TryGetValue(scheduledJobId, out cancellationTokenSource))
                {
                    cancellationTokenSource.Cancel();
                    _taskSettings.Tasks.TryRemove(scheduledJobId, out cancellationTokenSource);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to stop job with Id: {scheduledJobId}", ex);
            }
        }

        public async Task<string> UploadFile(IFormFile file, string name)
        {
            // create plugins directory 
            string pluginsDirectory = Path.Combine(Directory.GetCurrentDirectory(), "plugins");
            if (!Directory.Exists(pluginsDirectory))
            {
                Directory.CreateDirectory(pluginsDirectory);
            }

            // save file
            var path = Path.Combine(pluginsDirectory, file.FileName);
            using (var stream = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            string finalPath = path;
            if (file.ContentType.Contains("zip"))
            {
                finalPath = Path.Combine(pluginsDirectory, name);
                if (!Directory.Exists(finalPath))
                {
                    Directory.CreateDirectory(finalPath);
                }
                else
                {
                    Directory.Delete(finalPath, true);
                    Directory.CreateDirectory(finalPath);
                }
                ZipFile.ExtractToDirectory(path, finalPath);

                System.IO.File.Delete(path);//remove archive
            }
            //now get the path where the dll
            return $"{finalPath}";
        }

        public async Task<JobSessionFileResult> GetJobResultFile(Guid jobSessionId)
        {
            var jobSession = await _jobSessionRepository.GetById(jobSessionId);
            if(jobSession == null)
            {
                _logger.LogWarning($"There is no job sessions associated with Id: {jobSessionId}");
                throw new ArgumentException();
            }

            return new JobSessionFileResult()
            {
                FileAsBytes = File.ReadAllBytes(jobSession.ResultPath),
                ContentType = jobSession.ResultContentType
            };
        }

        public IEnumerable<JobSessionDto> GetAllJobSessionResults()
        {
            var jobSessions = _jobSessionRepository.GetJobSessionDtos();
            return jobSessions;
        }

        public async Task Update(Job job)
        {
            await _jobRepository.Update(job);
        }
    }
}
