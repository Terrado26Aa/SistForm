using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuthLogin.Models
{
    [Table("form_responses")]
    public class CFormResponse
    {
        [Key]
        //no se esta usando, pero se puede usar para mostrar el titulo de la pregunta en la lista de respuestas pendientes
        public int Id { get; set; }
        public int FormId { get; set; } //Que encuesta respondio
        public int UserId { get; set; } //Quien respondio
        public DateTime Date { get; set; } = DateTime.Now;
        public double? LatitudeA { get; set; }
        public double? LongitudeA { get; set; }
        public double? LatitudeB { get; set; }
        public double? LongitudeB { get; set; }
        public string? RoutePath { get; set; } //Podemos guardar el camino recorrido entre A y B como un string

        public List<CFormResponseDetail> Details { get; set; } = new List<CFormResponseDetail>();
    }

    //no se esta usando, pero se puede usar para mostrar el titulo de la pregunta en la lista de respuestas pendientes
    [Table("form_response_details")]
    public class CFormResponseDetail
    {
        [Key]
        public int Id { get; set; }
        public int ResponseId { get; set; } //Referencia a la respuesta padre
        public string QuestionTitle { get; set; } //Guardamos la pregunta por si cambia el form original
        public string Answer { get; set; } //Lo que el usuario escribio o seleccionó

        [ForeignKey ("ResponseId")]
        public CFormResponse CFormResponse { get; set; }
    }
}
