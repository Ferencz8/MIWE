using MIWE.Data;
using MIWE.Data.Entities;
using ProtoBuf.Grpc;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Text;

namespace MIWE.Core.Interfaces
{
    [ServiceContract]
    public interface IJobReceiver
    {

        void ReceiveJob(Job scheduledJob, CallContext callContext = default);

        void ReceiveJobSchedule(JobSchedule jobSchedule, CallContext callContext = default);
    }
}
