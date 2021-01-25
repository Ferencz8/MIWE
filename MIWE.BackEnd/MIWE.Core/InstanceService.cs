using MIWE.Core.Interfaces;
using MIWE.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using MIWE.Data.Services.Interfaces;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Http;

namespace MIWE.Data.Services
{
    public class InstanceService : IInstanceService
    {
        private IInstanceRepository _instanceRepository;

        public InstanceService(IInstanceRepository instanceRepository)
        {
            _instanceRepository = instanceRepository;
        }

        public int GetCurrentInstanceId()
        {
            string ip = GetCurrentExternalIP();
            var instance = _instanceRepository.GetAll(n => n.IP == ip).FirstOrDefault();
            return instance.Id;
        }

        public Instance GetCurrentInstance()
        {
            string ip = GetCurrentExternalIP();
            var instance = _instanceRepository.GetAll(n => n.IP == ip).FirstOrDefault();
            return instance;
        }

        public string GetAvailableInstanceIP()
        {
            string availableIP = string.Empty;
            int index = 1;
            do
            {
                availableIP = _instanceRepository.GetAvailableInstanceIP();
                Thread.Sleep(TimeSpan.FromSeconds(10 * index));
                index++;

            } while (string.IsNullOrEmpty(availableIP) && index < 10);
            return availableIP;
        }

        public bool IsMasterRegistered()
        {
            return _instanceRepository.IsMasterRegistered();
        }

        public async Task<int> RegisterInstance(bool isMaster)
        {
            string currentIp = GetCurrentExternalIP();
            //instance exists
            var existingInstance = _instanceRepository.GetAll(n => n.IP == currentIp).FirstOrDefault();
            if (existingInstance != null)
            {
                if (existingInstance.IsDown)
                {
                    existingInstance.IsDown = false;
                    existingInstance.IsMaster = isMaster;
                    await _instanceRepository.Update(existingInstance);
                }
                return existingInstance.Id;
            }
            else
            { 
                //adding instance
                var newInstance = await _instanceRepository.Create(new Instance()
                {
                    IP = currentIp,
                    IsAvailable = true,
                    IsDown = false,
                    IsMaster = isMaster
                });
                return newInstance.Id;
            }
        }

        private string GetCurrentExternalIP()
        {
            try
            {
                string externalIP;
                externalIP = (new WebClient()).DownloadString("http://checkip.dyndns.org/");
                externalIP = (new Regex(@"\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}"))
                             .Matches(externalIP)[0].ToString();
                if (externalIP.Contains("85.204.6.250")) return "https://localhost:8008";
                return externalIP;
            }
            //            catch { return "85.204.6.250"; }
            catch { return "https://localhost:8008"; }
        }

        public void CheckInstanceAvailability()
        {
            var result = IsCPUThresholdReached();
            MarkInstanceAvailable(result);
        }

        public void MarkInstanceAvailable(bool isAvailable = false)
        {
            string currentIp = GetCurrentExternalIP();
            _instanceRepository.MarkInstanceAvailable(currentIp, isAvailable);
        }

        public void MarkInstanceDown(bool isdown = false)
        {
            string currentIp = GetCurrentExternalIP();
            _instanceRepository.MarkInstanceDown(currentIp, isdown);
        }

        public bool IsCPUThresholdReached()
        {
            var currentProc = Process.GetCurrentProcess();
            string name = currentProc.ProcessName;
            PerformanceCounter total_cpu = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            PerformanceCounter process_cpu = new PerformanceCounter("Process", "% Processor Time", name);

            float totcalCpuValue = 0;
            do
            {
                totcalCpuValue = total_cpu.NextValue();
            } while (totcalCpuValue == 0 || totcalCpuValue == 100);

            return totcalCpuValue > 50;//TODO:: use property CPUThreshold from entity
        }

        public Instance GetMasterInstance()
        {
            return _instanceRepository.GetAll(n => n.IsMaster).FirstOrDefault();
        }

        public async Task<bool> PoolMasterAvailability(Instance masterInstance)
        {
            int failed = 0;
            do
            {
                try
                {
                    using (HttpClient client = new HttpClient())
                    {
                        HttpResponseMessage response = await client.GetAsync($"{masterInstance.IP}/hc");
                        response.EnsureSuccessStatusCode();
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    failed++;
                }
            }
            while (failed < 3);
            return false;
        }
    }
}
