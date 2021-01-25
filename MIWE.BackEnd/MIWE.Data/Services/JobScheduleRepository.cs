using Microsoft.EntityFrameworkCore;
using MIWE.Data.Dtos;
using MIWE.Data.Entities;
using MIWE.Data.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace MIWE.Data.Services
{
    public class JobScheduleRepository : GenericRepository<JobSchedule>, IJobScheduleRepository
    {
        public JobScheduleRepository(WorkerContext context) : base(context)
        {

        }

        public IEnumerable<JobScheduleLastSessionDto> GetJobsScheduledToRun()
        {
            var results = _dbContext.Set<JobScheduleLastSessionDto>().FromSqlRaw(@"SELECT js.*, jss.DateStart as LastSessionDateStart FROM JobSchedules js
LEFT JOIN JobSessions jss ON jss.EntityId = js.Id
INNER JOIN (SELECT EntityId, MAX(DateStart) AS DateStart FROM JobSessions GROUP BY EntityId) jss2 ON jss2.EntityId = js.Id AND jss2.DateStart = jss.DateStart
WHERE js.IsRunning = 0 
");

            return results;
        }

        public IEnumerable<JobSchedulePipelineDto> GetJobSchedulesWithPipeline()
        {
            var results = _dbContext.Set<JobSchedulePipelineDto>().FromSqlRaw(@"
SELECT 
js.Id,
j.Name + ' -> ' + j2.Name AS Pipeline,
js.Scheduling
FROM JobSchedules js
INNER JOIN Jobs j ON j.Id = js.MainJob
INNER JOIN Jobs j2 ON j2.Id = SUBSTRING(js.NextJobs, 1, LEN(js.NextJobs) - 1)");

            return results;
        }
    }
}
