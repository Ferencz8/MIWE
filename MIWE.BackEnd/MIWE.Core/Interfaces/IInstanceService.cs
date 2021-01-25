using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MIWE.Data.Services
{
    public interface IInstanceService
    {
        int GetCurrentInstanceId();

        Instance GetCurrentInstance();

        string GetAvailableInstanceIP();

        bool IsMasterRegistered();

        Task<int> RegisterInstance(bool isMaster);

        void CheckInstanceAvailability();

        void MarkInstanceAvailable(bool isAvailable = false);

        void MarkInstanceDown(bool isdown = false);

        bool IsCPUThresholdReached();

        Instance GetMasterInstance();

        Task<bool> PoolMasterAvailability(Instance masterInstance);
    }
}
