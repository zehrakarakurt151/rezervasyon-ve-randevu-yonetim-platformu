namespace rezervasyonapi.Models
{
    public class Service : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int DurationMinutes { get; set; } // Hizmetin süresi (Dakika)
        public int BufferTimeMinutes { get; set; } // İki randevu arasındaki hazırlık/temizlik süresi

        // İlişki Anahtarları
        public int ServiceProviderId { get; set; }

        // İlişkiler (Çok:1)
        public ServiceProvider ServiceProvider { get; set; }
        // İlişkiler (1:Çok)
        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    }
}
