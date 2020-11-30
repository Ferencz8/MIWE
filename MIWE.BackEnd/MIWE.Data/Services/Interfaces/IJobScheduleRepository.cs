using MIWE.Data.Dtos;
using MIWE.Data.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace MIWE.Data.Services.Interfaces
{
    public interface IJobScheduleRepository : IGenericRepository<JobSchedule>
    {

        public IEnumerable<JobScheduleDto> GetJobsScheduledToRun();
    }
}
