using Microsoft.AspNetCore.Mvc.ViewEngines;

namespace rezervasyonapi.Models
{
    public class Appointment : BaseEntity
    {
        public DateTimeOffset StartTime { get; set; }
        public DateTimeOffset EndTime { get; set; } 
        public decimal TotalPrice { get; set; } 

        public AppointmentStatus Status { get; set; }
        public string? CustomerNotes { get; set; }
        public string? ProviderCancellationReason { get; set; } // İptal nedenleri için

        // İlişki Anahtarları
        public int CustomerId { get; set; }
        public int ServiceProviderId { get; set; }
        public int ServiceId { get; set; }

        // İlişkiler (Çok:1)
        public Customer Customer { get; set; }
        public ServiceProvider ServiceProvider { get; set; }
        public Service Service { get; set; }

        // İlişkiler (1:1)
        public Review Review { get; set; } // Bu randevu için yapılan inceleme (zorunlu değil)
    }

    public enum AppointmentStatus
    {
        Pending, // Beklemede (Ödeme/Onay bekleniyor olabilir)
        Confirmed,
        CancelledByCustomer,
        CancelledByProvider,
        Completed,
        NoShow // Müşteri gelmedi
    }
}
