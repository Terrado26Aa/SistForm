using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace AuthLogin.Models
{
    [Table ("form_elements")] //Nombre de la tabla en MySQL
    public class CFormElement
    {
        [Key]
        public int Id { get; set; }
        public string Type { get; set; }
        public string Title { get; set; }

        //Clave foranea para conectar con el formulario padre
        public int FormId { get; set; }

        [JsonIgnore] //Evita ciclos infinitos al serializar
        [ForeignKey("FormId")]
        public CForm Form { get; set; }
    }
}
