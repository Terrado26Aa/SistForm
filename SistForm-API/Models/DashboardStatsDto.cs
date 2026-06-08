namespace SistFormAPI.Models
{
    public class DashboardStatsDto
    {
        public int TotalForms { get; set; }
        public int TotalUsers { get; set; }
        public int TotalResponses { get; set; }
        public List<FormSummaryDto> RecentForms { get; set; } = new List<FormSummaryDto>();
    }

    public class FormSummaryDto
    {
        public int IdForm { get; set; }
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public DateTime CreationDate { get; set; }
    }
}
