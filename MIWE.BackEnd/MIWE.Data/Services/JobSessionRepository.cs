using Microsoft.EntityFrameworkCore;
using MIWE.Data.Dtos;
using MIWE.Data.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace MIWE.Data.Services
{
    public class JobSessionRepository : GenericRepository<JobSession>, IJobSessionRepository
    {

        public JobSessionRepository(WorkerContext context) : base(context)
        {

        }

        public IEnumerable<JobSessionDto> GetJobSessionDtos()
        {
            var results = _dbContext.Set<JobSessionDto>().FromSqlRaw(@"SELECT 
                                                js.Id,
                                                js.DateStart,
                                                js.DateEnd,
                                                i.IP AS Instance,
                                                js.IsSuccess,
                                                js.ResultContentType,
                                                j.Name + ' -> ' + j2.Name AS JobPipeline
                                                FROM JobSessions js
                                                INNER JOIN Instances i ON i.Id = js.InstanceId
                                                INNER JOIN JobSchedules jsch ON jsch.Id = js.EntityId
                                                INNER JOIN Jobs j ON j.Id = jsch.MainJob
                                                INNER JOIN Jobs j2 ON j2.Id = SUBSTRING(jsch.NextJobs, 1, LEN(jsch.NextJobs) - 1)
                                                ORDER BY js.DateStart DESC");
            return results;

        }
    }
}
