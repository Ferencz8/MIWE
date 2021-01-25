using MIWE.Data.Entities;
using ProtoBuf;
using System;
using System.ComponentModel.DataAnnotations;

namespace MIWE.Data
{
    [ProtoContract]
    public class Job
    {
        [ProtoMember(1, DataFormat = DataFormat.FixedSize)]
        [Key]
        public Guid Id { get; set; }

        [ProtoMember(2, DataFormat = DataFormat.Default)]
        [Required]
        public string Name { get; set; }
                
        [ProtoMember(3, DataFormat = DataFormat.Default)]
        public DateTime DateAdded { get; set; }

        [ProtoMember(4, DataFormat = DataFormat.Default)]
        [Required]
        public string PluginPath { get; set; }

        [ProtoMember(5, DataFormat = DataFormat.Default)]
        public OSType OSType { get; set; }

        [ProtoMember(6, DataFormat = DataFormat.Default)]
        public bool IsActive { get; set; }

        [ProtoMember(7, DataFormat = DataFormat.Default)]
        public bool IsRunning { get; set; }

        [ProtoMember(8, DataFormat = DataFormat.Default)]
        public PluginType PluginType { get; set; }

        [ProtoMember(9, DataFormat = DataFormat.Default)]
        public string Description { get; set; }

        [ProtoMember(10, DataFormat = DataFormat.Default)]
        public DateTime? DateModified { get; set; }
    }
}
