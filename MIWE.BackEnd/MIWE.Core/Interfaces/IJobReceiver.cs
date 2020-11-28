using MIWE.Data;
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
    }
}
