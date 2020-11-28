using Microsoft.EntityFrameworkCore;
using MIWE.Data.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIWE.Data.Services
{
    public class InstanceRepository : GenericRepository<Instance>, IInstanceRepository
    {
        public InstanceRepository(WorkerContext context) : base(context)
        {

        }

        public string GetAvailableInstanceIP()
        {
            var availableInstance = _dbContext.Instances.FromSqlRaw("SELECT * FROM Instances WHERE IsAvailable = 1 AND IsMaster = 0")
                    .FirstOrDefault();
            return availableInstance?.IP;
        }

        public bool IsMasterRegistered()
        {
            var isMasterRegistered = _dbContext.Instances
                    .FromSqlRaw("SELECT * FROM Instances WHERE IsMaster = 1 AND IsDown = 0")
                    .Any();
            return isMasterRegistered;
        }

        public async Task MarkInstanceAvailable(string currentIp, bool isAvailable = false)
        {
            var currentInstance = _dbContext.Instances.FirstOrDefault(n => n.IP == currentIp);
            currentInstance.IsAvailable = isAvailable;
            await _dbContext.SaveChangesAsync();
        }

        public async Task MarkInstanceDown(string currentIp, bool isdown = false)
        {
            var currentInstance = _dbContext.Instances.FirstOrDefault(n => n.IP == currentIp);
            currentInstance.IsDown = isdown;
            currentInstance.IsMaster = false;
            await _dbContext.SaveChangesAsync();
        }
    }
}
