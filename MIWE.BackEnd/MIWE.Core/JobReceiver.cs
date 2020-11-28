using MIWE.Core.Interfaces;
using MIWE.Data;
using MIWE.Data.Services;
using ProtoBuf.Grpc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MIWE.Core
{
    public class JobReceiver : IJobReceiver
    {
        private IInstanceService _instanceService;
        private IJobExecuter _jobExecuter;

        public JobReceiver(IInstanceService instanceService, IJobExecuter jobExecuter)
        {
            _instanceService = instanceService;
            _jobExecuter = jobExecuter;
        }

        public void ReceiveJob(Job job, CallContext callContext = default) 
        {
            //check for CPU and mark availability
            _instanceService.CheckInstanceAvailability();

            _jobExecuter.RunJob(_instanceService.GetCurrentInstanceId(), job.Id, job.PluginPath);
        }
    }
}
