using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forms.Models
{
    public class FormDto
    {
        public int IdForm { get; set; }
        public int IdUser { get; set; }
        [Required]
        public string Title { get; set; }
        public string? Description { get; set; }

        public List<FormElementDto> Elements { get; set; } = new List<FormElementDto>();
    }

    public class FormElementDto
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public string Title { get; set; }
        public string Options { get; set; } // Solo se usará para elementos de tipo "select", "checkbox" o "radio"
    }
}
