using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MIWE.Core.Interfaces;
using MIWE.Data;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Mvc;
using ProtoBuf.Grpc.Client;
using MIWE.Data.Services;
using MIWE.Data.Entities;

namespace MIWE.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private IInstanceService _instanceService;
        private IJobExecuter _jobExecuter;
        public ValuesController(IInstanceService instanceService, IJobExecuter jobExecuter)
        {
            _instanceService = instanceService;
            _jobExecuter = jobExecuter;
        }

        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            var tokenSource = new CancellationTokenSource();
            var token = tokenSource.Token;
            Task jobRunnerTask = Task.Factory.StartNew(() =>
            {
                while (!token.IsCancellationRequested)
                {
                    RunJobs(token);

                    Thread.Sleep(TimeSpan.FromSeconds(10));
                }

                if (token.IsCancellationRequested)
                {
                    //Console.WriteLine("Task {0} was cancelled before it got started.",
                    //              taskNum);
                    token.ThrowIfCancellationRequested();
                }
            }, token);


            try
            {
                jobRunnerTask.Wait();
            }
            catch (OperationCanceledException operationCanceledEx)
            {
                //log
            }
            catch (Exception genericException)
            {
                //log
            }
            finally
            {
                tokenSource.Dispose();
            }

            return "";
        }

        private void RunJobs(CancellationToken token)
        {
            var allJobsRan = _jobExecuter.RunAllJobs();
            if (!allJobsRan)
            {
                string ip = _instanceService.GetAvailableInstanceIP();
                if (string.IsNullOrEmpty(ip))
                    return;

                var jobs = _jobExecuter.GetAllJobs();
                //call instance to run a job

                try
                {
                    GrpcClientFactory.AllowUnencryptedHttp2 = true;
                    var channel = GrpcChannel.ForAddress($"https://localhost:8009");

                    var jobReceiverService = channel.CreateGrpcService<IJobReceiver>();
                    jobReceiverService.ReceiveJob(jobs.ElementAt(0));                    
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
