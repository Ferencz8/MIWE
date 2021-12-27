using MIWE.Data.Dtos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MIWE.Data.Services.Interfaces
{
    public interface IJobSessionRepository : IGenericRepository<JobSession>
    {
        IEnumerable<JobSessionDto> GetJobSessionDtos();

        Task<int> GetJobSessionsCount();
    }
}
