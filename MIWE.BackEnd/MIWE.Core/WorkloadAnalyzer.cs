using MIWE.Core.Interfaces;
using MIWE.Data.Dtos;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MIWE.Core
{
    public class WorkloadAnalyzer : IWorkloadAnalyzer
    {
        public IEnumerable<Guid> ScanJobsAwaitingToBeRun(IEnumerable<JobScheduleLastSessionDto> jobsToBeRun)
        {
            List<Guid> ids = new List<Guid>();
            for (int i = 0; i < jobsToBeRun.Count(); i++)
            {
                var job = jobsToBeRun.ElementAt(i);
                CronExpression expression = new CronExpression(job.Scheduling);
                expression.TimeZone = TimeZoneInfo.Utc;
                DateTimeOffset? nextFireUTCTime = expression.GetNextValidTimeAfter(DateTime.UtcNow);
                if (DateTime.Compare(DateTime.UtcNow, nextFireUTCTime.Value.UtcDateTime) < 0) //T1 < T2 -> the time has not yet come
                {
                    continue; //skip
                }
                if (job.LastSessionDateStart.HasValue && DateTime.Compare(job.LastSessionDateStart.Value.AddMinutes(5), nextFireUTCTime.Value.UtcDateTime) < 0)// the job already ran
                {
                    continue;//skip
                }
                ids.Add(job.Id);
            }
            return ids;
        }
    }
}
