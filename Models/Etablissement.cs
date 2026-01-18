using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ufas1Forms.Models
{
    public class Etablissement
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]   
        public int Id { get; set; }

        [MaxLength(255)]
        [Required]
        public string  Nom { get; set; } = string.Empty;

        [MaxLength(255)]
        public string  Adresse { get; set; } = string.Empty;
    }
}