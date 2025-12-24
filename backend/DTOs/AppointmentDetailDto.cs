namespace RezervasyonAPI.DTOs
{
    public class AppointmentDetailDto
    {
        public int Id { get; set; }

        
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public string ServiceName { get; set; }
        public decimal Price { get; set; }
        public string Status { get; set; }
        public string ProviderName { get; set; }
        public string CustomerName { get; set; }
        public string CustomerPhone { get; set; }
    }
}