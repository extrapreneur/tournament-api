using System.ComponentModel.DataAnnotations;

namespace MyBackend.Models
{
    public class Match
    {
        [Key]
        public int Id { get; set; }

        public Player Player1 { get; set; }

        public Player Player2 { get; set; }

        public int Round { get; set; }

        public Player Winner { get; set; }

        public bool Decided { get; set; }
    }
}