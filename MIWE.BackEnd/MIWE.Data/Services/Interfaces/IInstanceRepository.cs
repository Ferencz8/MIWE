using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MIWE.Data.Services.Interfaces
{
    public interface IInstanceRepository : IGenericRepository<Instance>
    {
        string GetAvailableInstanceIP();

        bool IsMasterRegistered();

        Task MarkInstanceAvailable(string currentIp, bool isAvailable = false);

        Task MarkInstanceDown(string currentIp, bool isdown = false);

        Task<int> CheckOrChangeMaster(int currentId);

        Task ChangeMaster(int currentId);
    }
}
