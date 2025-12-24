namespace RezervasyonAPI.DTOs
{
    public class CreateAppointmentDto
    {
        public int ServiceProviderId { get; set; } // Hangi dükkan?
        public int ServiceId { get; set; }         // Hangi hizmet?
        public DateTime Date { get; set; }      // Örn: "2025-11-25" (Takvimden gelen)
        public string Time { get; set; }    // Ne zaman? (Örn: 2025-11-25T09:00:00)
    }
}