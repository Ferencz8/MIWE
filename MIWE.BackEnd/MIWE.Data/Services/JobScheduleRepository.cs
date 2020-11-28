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
    }
}
