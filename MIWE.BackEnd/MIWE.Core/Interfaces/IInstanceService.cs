using System;
using System.Collections.Generic;
using System.Text;

namespace MIWE.Data.Services
{
    public interface IInstanceService
    {
        int GetCurrentInstanceId();

        Instance GetCurrentInstance();

        string GetAvailableInstanceIP();

        bool IsMasterRegistered();

        void RegisterInstance(bool isMaster);

        void CheckInstanceAvailability();

        void MarkInstanceAvailable(bool isAvailable = false);

        void MarkInstanceDown(bool isdown = false);

        bool IsCPUThresholdReached();
    }
}
