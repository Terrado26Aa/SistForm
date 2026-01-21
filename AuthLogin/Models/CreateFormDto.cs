namespace AuthLogin.Models
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
        public string Title { get; set; }
        public string Type { get; set; }
    }
}
