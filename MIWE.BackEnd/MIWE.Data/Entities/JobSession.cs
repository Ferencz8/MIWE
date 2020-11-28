using MIWE.Data.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MIWE.Data
{
    public class JobSession
    {
        [Key]
        public Guid Id { get; set; }

        public DateTime DateStart { get; set; }

        public DateTime? DateEnd { get; set; }

        public int InstanceId { get; set; }

        public bool IsSuccess { get; set; }

        public Guid EntityId { get; set; }

        public string ResultPath { get; set; }

        public string ResultContentType { get; set; }
    }
}
