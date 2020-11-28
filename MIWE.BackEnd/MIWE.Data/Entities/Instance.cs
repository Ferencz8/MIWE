using System.ComponentModel.DataAnnotations;

namespace MIWE.Data
{
    public class Instance
    {
        [Key]
        public int Id { get; set; }

        public string IP { get; set; }

        public int CPUThreshold { get; set; }

        public bool IsAvailable { get; set; }

        public bool IsMaster { get; set; }

        public bool IsDown { get; set; }
    }
}
