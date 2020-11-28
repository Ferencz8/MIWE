using Microsoft.EntityFrameworkCore;
using MIWE.Data.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MIWE.Data.Services
{
    public class JobRepository : GenericRepository<Job>, IJobRepository
    {
        public JobRepository(WorkerContext context)
         : base(context)
        {

        }

        public async Task MarkStopped(int instanceId)
        {
            await _dbContext.Database.ExecuteSqlRawAsync(@"UPDATE Jobs SET IsRunning = 0
                                                   WHERE Id IN (SELECT EntityId FROM Jobs
                                                   WHERE DateEnd = NULL AND InstanceId = {0})", instanceId);
        }
    }
}
