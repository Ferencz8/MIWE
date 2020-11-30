using ProtoBuf;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MIWE.Data.Entities
{
    [ProtoContract]
    public class JobSchedule
    {
        [Key]
        [ProtoMember(1, DataFormat = DataFormat.FixedSize)]
        public Guid Id { get; set; }

        [ProtoMember(2, DataFormat = DataFormat.Default)]
        public string Scheduling { get; set; }

        [ProtoMember(3, DataFormat = DataFormat.Default)]
        public Guid MainJob { get; set; }

        [ProtoMember(4, DataFormat = DataFormat.Default)]
        public string NextJobs { get; set; }

        [ProtoMember(5, DataFormat = DataFormat.Default)]
        public bool IsRunning { get; set; }
    }
}
