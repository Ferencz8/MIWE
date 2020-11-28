using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace MIWE.Core
{
    //https://stackoverflow.com/questions/49813628/run-a-background-task-from-a-controller-action-in-asp-net-core-2
    public class TaskSettings
    {
        public TaskSettings()
        {
            Tasks = new ConcurrentDictionary<Guid, CancellationTokenSource>();
        }
        public ConcurrentDictionary<Guid, CancellationTokenSource> Tasks { get; set; }
    }
}
