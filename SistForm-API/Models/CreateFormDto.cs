namespace SistFormAPI.Models
{
    public class CreateFormDto
    {
        public int IdUser { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }

        //Lista de elementos que viene del celular
        public List<FormElementDto> Elements { get; set; } = new List<FormElementDto>();
    }

    //Clase auxiliar para recibir cada elemento
    public class FormElementDto
    {
        public int? Id { get; set; }
        public string Title { get; set; }
        public string Type { get; set; }
        public string? Options { get; set; } // Solo se usará para elementos de tipo "select", "checkbox" o "radio"

        //no se esta usando, pero se puede usar para limitar el número de opciones seleccionables en elementos de tipo "checkbox"
        public int MaxSelections { get; set; }
    }
    public class FormAssignmentDto
    {
        public int FormId { get; set; }
        public int UserId { get; set; }
    }
}
