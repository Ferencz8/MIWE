using MIWE.Data.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace MIWE.Data.Services.Interfaces
{
    public interface IJobSessionRepository : IGenericRepository<JobSession>
    {
        IEnumerable<JobSessionDto> GetJobSessionDtos();
    }
}
