using MIWE.Data;
using MIWE.Data.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MIWE.Core.Interfaces
{
    public interface IJobExecuter
    {

        IEnumerable<JobSchedule> GetAllJobsScheduled();

        Task<bool> RunAllJobSchedules(CancellationToken? cancellationToken);

        Task RunJob(int instanceId, Guid jobId, string crawlPath, CancellationToken? cancellationToken = null);

        Task RunScheduledJob(int instanceId, Guid jobScheduleId, CancellationToken? cancellationToken = null);
    }
}
