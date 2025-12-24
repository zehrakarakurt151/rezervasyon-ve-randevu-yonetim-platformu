namespace rezervasyonapi.Models
{
    public class WorkSchedule : BaseEntity
    {
        public DayOfWeek DayOfWeek { get; set; }
        public TimeSpan StartTime { get; set; } // Örn: 09:00:00
        public TimeSpan EndTime { get; set; } // Örn: 18:00:00
        public bool IsWorking { get; set; } // O gün çalışıp çalışmadığı

        // İlişki Anahtarları
        public int ServiceProviderId { get; set; }

        // İlişkiler (Çok:1)
        public ServiceProvider ServiceProvider { get; set; }
    }
}
