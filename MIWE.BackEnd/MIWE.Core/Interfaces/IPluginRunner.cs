using MIWE.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MIWE.Core.Interfaces
{
    public interface IPluginRunner
    {
        bool Run(string crawlPath, CancellationToken? cancellationToken);

        //bool Run(string crawlerPluginPath, string processorPluginPath = null, string merchantName = null);

        Task<bool> Run(PluginRunningParameters pluginRunningParameters, CancellationToken? cancellationToken);
    }
}
