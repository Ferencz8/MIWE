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

        public IEnumerable<JobScheduleDto> GetJobsScheduledToRun()
        {
            var results = _dbContext.Set<JobScheduleDto>().FromSqlRaw(@"SELECT js.*, jss.DateStart as LastSessionDateStart FROM JobSchedules js
INNER JOIN JobSessions jss ON jss.EntityId = js.Id
WHERE js.IsRunning = 0 AND jss.IsSuccess = 1
");

            return results;
        }
    }
}
