using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MIWE.Data.Dtos
{
    [NotMapped]
    public class JobScheduleLastSessionDto
    {
        public Guid Id { get; set; }

        public string Scheduling { get; set; }

        public Guid MainJob { get; set; }

        public string NextJobs { get; set; }

        public bool IsRunning { get; set; }

        public DateTime? LastSessionDateStart { get; set; }
    }
}
