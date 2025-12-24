namespace rezervasyonapi.DTOs
{
    public class CreateServiceDto
    {
        public string Name { get; set; } // Örn: Saç Kesimi
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int DurationMinutes { get; set; } // 30 dk
    }
}
