using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MIWE.Common;
using MIWE.Core.Interfaces;
using MIWE.Core.Models;
using MIWE.Data;
using MIWE.Data.Dtos;
using MIWE.Data.Services;
using MIWE.Data.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
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
        private IAzureBlobRepository _azureBlobRepository;
        public JobService(IJobExecuter jobExecuter, TaskSettings taskSettings, ILogger<JobService> logger,
            IJobRepository jobRepository, IInstanceRepository instanceRepository, IJobSessionRepository jobSessionRepository,
            IServiceScopeFactory serviceScopeFactory, IAzureBlobRepository azureBlobRepository)
        {
            _jobExecuter = jobExecuter;
            _taskSettings = taskSettings;
            _logger = logger;
            _jobRepository = jobRepository;
            _intanceRepository = instanceRepository;
            _jobSessionRepository = jobSessionRepository;
            _serviceScopeFactory = serviceScopeFactory;
            _azureBlobRepository = azureBlobRepository;
        }

        public async Task<Guid> Add(Job job)
        {
            job.DateAdded = DateTime.UtcNow;
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
                _logger.LogWarning($"No available job found for Id: {jobId}");
                return;
            }

            string crawlPath = PluginHelper.GetLocalPluginPath(job.Name, job.PluginPath, job.DateModified);

            var tokenSource = new CancellationTokenSource();
            var token = tokenSource.Token;
            Task.Factory.StartNew(async () =>
            {
                try
                {
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        var jobExecuterService = scope.ServiceProvider.GetRequiredService<IJobExecuter>();
                        //await jobExecuterService.RunJob(instanceId.Value, jobId, crawlPath, token);
                        await jobExecuterService.RunJobWithCsvProcessor(instanceId.Value, jobId, crawlPath, token);

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

        public bool IsPluginAvailableLocaly(string azurePath)
        {
            string filneName = azurePath.Split("/").Last().Split(".").Last();
            string pluginsDirectory = Path.Combine(Directory.GetCurrentDirectory(), "plugins");
            string finalPath = Path.Combine(pluginsDirectory, filneName);
            return Directory.Exists(finalPath);
            //TODO:: compare the dates to see if a newer version needs to bbe downloaded
        }

        //public string GetLocalPluginPath(string pluginName, string azurePath, DateTime? lastModifiedDate)
        //{
        //    //var elements = azurePath.Split("/").Last().Split(".");            
        //    //string filneName = string.Join(".", elements.Take(elements.Length - 1));
        //    string pluginsDirectory = Path.Combine(Directory.GetCurrentDirectory(), "plugins");
        //    string finalPath = Path.Combine(pluginsDirectory, pluginName);
        //    if (!Directory.Exists(finalPath))
        //    {
        //        using (var client = new WebClient())
        //        {
        //            string pathWithFile = Path.Combine(finalPath, azurePath.Split("/").Last());
        //            client.DownloadFile(azurePath, pathWithFile);

        //            PluginHelper.ExtractArchive(pluginName, pluginsDirectory, finalPath);
        //        }                
        //    }
        //    else
        //    {
        //        DateTime lastWriteTime = System.IO.File.GetLastWriteTimeUtc(finalPath);
        //        //TODO:: compare with buffer of x minutes
        //        if (lastModifiedDate.HasValue && lastWriteTime < lastModifiedDate.Value)//local version is out of date
        //        {
        //            using (var client = new WebClient())
        //            {
        //                string pathWithFile = Path.Combine(finalPath, azurePath.Split("/").Last());
        //                client.DownloadFile(azurePath, pathWithFile);

        //                PluginHelper.ExtractArchive(pluginName, pluginsDirectory, finalPath);
        //            }
        //        }
        //    }
        //    return finalPath;
        //}

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
            try
            {
                // create plugins directory 
                string pluginsDirectory = Path.Combine(Directory.GetCurrentDirectory(), "plugins");
                if (!Directory.Exists(pluginsDirectory))
                {
                    Directory.CreateDirectory(pluginsDirectory);
                }

                string azurePath = string.Empty;
                // save file
                var path = Path.Combine(pluginsDirectory, file.FileName);
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await file.CopyToAsync(stream);

                    stream.Seek(0, SeekOrigin.Begin);

                    azurePath = await _azureBlobRepository.UploadPluginAsync(file.FileName, stream);// save to azure also
                }

                //string finalPath = path;
                //if (file.ContentType.Contains("zip"))
                //{
                //    finalPath = PluginHelper.ExtractArchive(name, pluginsDirectory, path);
                //}
                                
                //now get the path where the dll
                return $"{azurePath}";
            }
            catch(Exception exception)
            {
                _logger.LogError($"Failed to upload file with name {name}", exception);
                throw;
            }
        }

        //private string ExtractArchive(string name, string pluginsDirectory, string path)
        //{
        //    string finalPath = Path.Combine(pluginsDirectory, name);
        //    if (!Directory.Exists(finalPath))
        //    {
        //        Directory.CreateDirectory(finalPath);
        //    }
        //    else
        //    {
        //        Directory.Delete(finalPath, true);
        //        Directory.CreateDirectory(finalPath);
        //    }
        //    ZipFile.ExtractToDirectory(path, finalPath);

        //    File.Delete(path);//remove archive
        //    return finalPath;
        //}

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
            job.DateModified = DateTime.UtcNow;
            await _jobRepository.Update(job);
        }
    }
}
