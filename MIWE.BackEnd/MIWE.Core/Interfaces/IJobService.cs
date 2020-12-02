using Microsoft.AspNetCore.Http;
using MIWE.Core.Models;
using MIWE.Data;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MIWE.Core.Interfaces
{
    public interface IJobService
    {

        Task<Guid> Add(Job scheduledJob);

        Task Update(Job scheduledJob);

        IEnumerable<Job> Get();

        Task<Job> GetById(Guid scheduledJobId);

        void StartJob(Guid scheduledJobId);

        void StopJob(Guid scheduledJobId);

        Task<string> UploadFile(IFormFile file, string name);

        Task MarkStopped(int instanceId);

        Task<JobSessionFileResult> GetJobResultFile(Guid jobSessionId);
    }
}
