using MIWE.Data;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MIWE.Core.Interfaces
{
    public interface IJobExecuter
    {

        IEnumerable<Job> GetAllJobs();

        Task<bool> RunAllJobs();

        Task<bool> RunAllJobSchedules();

        Task RunJob(int instanceId, Guid jobId, string crawlPath, CancellationToken? cancellationToken = null);
    }
}
