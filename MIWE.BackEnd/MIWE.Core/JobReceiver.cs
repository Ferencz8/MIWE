using Microsoft.Extensions.DependencyInjection;
using MIWE.Core.Interfaces;
using MIWE.Data;
using MIWE.Data.Entities;
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
        private IServiceScopeFactory _serviceScopeFactory;

        public JobReceiver(IInstanceService instanceService, IJobExecuter jobExecuter, IServiceScopeFactory serviceScopeFactory)
        {
            _instanceService = instanceService;
            _jobExecuter = jobExecuter;
            _serviceScopeFactory = serviceScopeFactory;
        }

        public void ReceiveJob(Job job, CallContext callContext = default) 
        {
            //check for CPU and mark availability
            _instanceService.CheckInstanceAvailability();

            _jobExecuter.RunJob(_instanceService.GetCurrentInstanceId(), job.Id, job.PluginPath);
        }

        public void ReceiveJobSchedule(JobSchedule jobSchedule, CallContext callContext = default)
        {
           // using (var scope = _serviceScopeFactory.CreateScope())
            {
                //check for CPU and mark availability
                _instanceService.CheckInstanceAvailability();

                _jobExecuter.RunScheduledJob(_instanceService.GetCurrentInstanceId(), jobSchedule.Id);
            }
        }
    }
}
