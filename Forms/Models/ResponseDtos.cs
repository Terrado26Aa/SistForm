using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forms.Models
{
    public class SubmitResponseDto
    {
        public int FormId { get; set; }
        public int UserId { get; set; }
        public List<ResponseDetailDto> Responses { get; set; }

        // Para identificar la respuesta localmente antes de enviarla al servidor
        public string LocalId { get; set; } = Guid.NewGuid().ToString();

        //Propiedades para mostrar en la lista de respuestas pendientes
        public string FormTitle { get; set; }
        public DateTime SaveAt { get; set; } = DateTime.Now; //Guarda la fecha y hora.
    }

    public class ResponseDetailDto
    {
        public int FormElementId { get; set; }
        //no esta siendo usada, pero se puede usar para mostrar el titulo de la pregunta en la lista de respuestas pendientes
        public string QuestionTitle { get; set; }
        public string Answer { get; set; }
    }
}
