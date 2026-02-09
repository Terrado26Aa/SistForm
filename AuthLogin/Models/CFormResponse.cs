using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuthLogin.Models
{
    [Table("form_responses")]
    public class CFormResponse
    {
        [Key]
        public int Id { get; set; }
        public int FormId { get; set; } //Que encuesta respondio
        public int UserId { get; set; } //Quien respondio
        public DateTime Date { get; set; } = DateTime.Now;

        public List<CFormResponseDetail> Details { get; set; } = new List<CFormResponseDetail>();
    }

    [Table("form_response_details")]
    public class CFormResponseDetail
    {
        [Key]
        public int Id { get; set; }
        public int ResponseId { get; set; } //Referencia a la respuesta padre
        public string QuestionTitle { get; set; } //Guardamos la pregunta por si cambia el form original
        public string Answer { get; set; } //Lo que el usuario escribio o seleccionó
    }
}
