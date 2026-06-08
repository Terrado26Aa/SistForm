namespace SistFormAPI.Models
{
    public class SubmitResponseDto
    {
        public int FormId { get; set; }
        public int UserId { get; set; }
        public List<ResponseDetailDto> Responses { get; set; } = new List<ResponseDetailDto>();

        public string? FormTitle { get; set; }
        public DateTime SaveAt { get; set; } = DateTime.Now;
        public double? LatitudeA { get; set; }
        public double? LongitudeA { get; set; }
        public double? LatitudeB { get; set; }
        public double? LongitudeB { get; set; }

        public List<TrackPoint>? TrackPoints { get; set; }
    }

    public class TrackPoint
    {
        public double Lat { get; set; }
        public double Lon { get; set; }
    }

    public class ResponseDetailDto
    {
        public int FormElementId { get; set; }
        public string? Question { get; set; }
        public string? Answer { get; set; }
    }
}
