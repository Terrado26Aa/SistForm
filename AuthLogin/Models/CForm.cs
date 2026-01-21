using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuthLogin.Models
{
    [Table("createforms")]
    public class CForm
    {
        [Key]
        public int IdForm { get; set; }
        public int IdUser { get; set; }
        [Required]
        public string Title { get; set; }
        public string? Description { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime LastModifiedDate { get; set; }

        //Esto crea la relación en Entity Framework
        public List<CFormElement> Elements { get; set; } = new List<CFormElement>();
    }
}
