using System;
using System.Collections.Generic;
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
    }

    public class ResponseDetailDto
    {
        public int FormElementId { get; set; }
        public string QuestionTitle { get; set; }
        public string Answer { get; set; }
    }
}
