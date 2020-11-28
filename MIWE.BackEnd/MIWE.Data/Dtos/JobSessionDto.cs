using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MIWE.Data.Dtos
{
    [NotMapped]
    public class JobSessionDto
    {

        public Guid Id { get; set; }

        public DateTime DateStart { get; set; }

        public DateTime? DateEnd { get; set; }

        public string Instance { get; set; }

        public bool IsSuccess { get; set; }

        public string JobPipeline { get; set; }

        public string ResultContentType { get; set; }
    }
}
