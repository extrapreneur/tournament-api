using System.ComponentModel.DataAnnotations;

namespace MyBackend.Models
{
    public class Player
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public required string Name { get; set; }

        [Required]
        public required string Email { get; set; }
    }
}