using System.ComponentModel.DataAnnotations;

namespace MyBackend.Models
{
    public class Match
    {
        [Key]
        public int Id { get; set; }

        public int? Player1Id { get; set; }

        public int? Player2Id { get; set; }

        public int? Round { get; set; }

        public int? WinnerId { get; set; }

        public bool Decided { get; set; }

        public Match? NextMatch { get; set; }
    }
}