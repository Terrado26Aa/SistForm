using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistFormAPI.Models
{
    [Table("form_assignments")]
    public class FormAssignment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int FormId { get; set; }

        [Required]
        public int UserId { get; set; }

        public DateTime AssignedDate { get; set; } = DateTime.Now;

        [ForeignKey("FormId")]
        public CForm? Form { get; set; }

        [ForeignKey("UserId")]
        public User? User { get; set; }
    }
}
