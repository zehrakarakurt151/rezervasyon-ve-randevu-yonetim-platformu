using Microsoft.AspNetCore.Mvc.ViewEngines;

namespace rezervasyonapi.Models
{
    public class ServiceProvider : ApplicationUser
    {
        public string BusinessName { get; set; } 
        public string Address { get; set; }
        public string Description { get; set; }
        public double AverageRating { get; set; } // Hesaplanan ortalama puan

        // İlişkiler (1:Çok)
        public ICollection<Service> Services { get; set; } = new List<Service>(); // Sunduğu hizmetler
        public ICollection<WorkSchedule> WorkSchedules { get; set; } = new List<WorkSchedule>(); // Genel çalışma programı
        public ICollection<BlockedSlot> BlockedSlots { get; set; } = new List<BlockedSlot>(); // Bloke zaman dilimleri
        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>(); // Aldığı randevular
        public ICollection<Review> ReviewsReceived { get; set; } = new List<Review>(); // Hakkında yapılan incelemeler
    }
}
