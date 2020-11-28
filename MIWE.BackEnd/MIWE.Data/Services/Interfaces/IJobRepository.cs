using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MIWE.Data.Services.Interfaces
{
    public interface IJobRepository : IGenericRepository<Job>
    {

        Task MarkStopped(int instanceId);
    }
}
