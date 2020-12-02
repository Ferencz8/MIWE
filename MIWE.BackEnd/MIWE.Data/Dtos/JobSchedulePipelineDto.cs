using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MIWE.Data.Dtos
{
    [NotMapped]
    public class JobSchedulePipelineDto
    {
        public Guid Id { get; set; }

        public string Pipeline { get; set; }

        public string Scheduling { get; set; }
    }
}
