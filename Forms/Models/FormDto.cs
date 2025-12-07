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
        public int IdUser { get; set; }
        [Required]
        public string Title { get; set; }
        public string? Description { get; set; }

        public List<FormElementDto> Elements { get; set; } = new List<FormElementDto>();
    }

    public class FormElementDto
    {
        public string Type { get; set; }
        public string Title { get; set; }
    }
}
