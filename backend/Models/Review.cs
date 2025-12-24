namespace rezervasyonapi.Models
{
    public class Review : BaseEntity
    {
        public int Rating { get; set; } // 1-5 arasında bir değer (Kısıtlama olmalı)
        public string Comment { get; set; }

        // İlişki Anahtarları
        public int ServiceProviderId { get; set; }
        public int CustomerId { get; set; }
        public int AppointmentId { get; set; }

        // İlişkiler (Çok:1)
        public ServiceProvider ServiceProvider { get; set; }
        public Customer Customer { get; set; }
        public Appointment Appointment { get; set; }
    }
}
