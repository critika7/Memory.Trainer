using System.Collections.Generic;

namespace LegoCityUnderCover.Trainer.Models
{
    public class MemAddress
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public List<string> Addresses { get; set; } = new List<string>();
    }
}