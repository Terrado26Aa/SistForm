using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace AuthLogin.Models
{
    [Table ("form_elements")] //Nombre de la tabla en MySQL
    public class CFormElement
    {
        [Key]
        //no se esta usando, pero se puede usar para mostrar el titulo de la pregunta en la lista de respuestas pendientes
        public int Id { get; set; }
        public string Type { get; set; }
        public string Title { get; set; }
        public string? Options { get; set; } // Solo se usará para elementos de tipo "select", "checkbox" o "radio"
        public int? MaxSelections { get; set; }

        //Clave foranea para conectar con el formulario padre
        public int FormId { get; set; }

        [JsonIgnore] //Evita ciclos infinitos al serializar
        [ForeignKey("FormId")]
        //no se esta usando, pero se puede usar para mostrar el titulo de la pregunta en la lista de respuestas pendientes
        public CForm Form { get; set; }
    }
}
