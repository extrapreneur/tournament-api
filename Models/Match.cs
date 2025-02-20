using System.ComponentModel.DataAnnotations;

namespace MyBackend.Models
{
    public class Match
    {
        [Key]
        public int Id { get; set; }

        public int PlayerId1 { get; set; }

        public int? PlayerId2 { get; set; }

        public int? Round { get; set; }

        public int? WinnerId { get; set; }

        public bool Decided { get; set; }
    }
}