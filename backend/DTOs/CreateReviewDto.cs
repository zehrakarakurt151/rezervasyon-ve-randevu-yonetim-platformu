namespace rezervasyonapi.DTOs
{
    public class CreateReviewDto
    {
        public int AppointmentId { get; set; }
        public int Rating { get; set; } // 1 ile 5 arası
        public string Comment { get; set; }
    }
}
