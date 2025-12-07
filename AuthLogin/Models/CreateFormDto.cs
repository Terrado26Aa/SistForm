namespace AuthLogin.Models
{
    public class CreateFormDto
    {
        public int IdUser { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
    }
}
