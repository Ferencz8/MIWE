using MIWE.Data.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace MIWE.Core.Interfaces
{
    public interface IWorkloadAnalyzer
    {

        IEnumerable<Guid> ScanJobsAwaitingToBeRun(IEnumerable<JobScheduleLastSessionDto> jobScheduleDtos);
    }
}
