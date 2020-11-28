using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MIWE.Data.Entities
{
    public class JobSchedule
    {
        [Key]
        public Guid Id { get; set; }

        public string Scheduling { get; set; }

        public Guid MainJob { get; set; }

        public string NextJobs { get; set; }

        public bool IsRunning { get; set; }
    }
}
