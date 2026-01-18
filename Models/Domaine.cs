using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ufas1Forms.Models
{
    public class Domaine
    {
        public int Id { get; set; }

        [MaxLength(255)]
        [Required]
        public string  Nom { get; set; } = string.Empty;

        public int FaculteId { get; set; }

        [ForeignKey("FaculteId")]
        public Faculte? Faculte { get; set; }
    }
}