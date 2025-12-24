namespace rezervasyonapi.DTOs
{
    namespace RezervasyonAPI.DTOs
    {
        public class WorkScheduleDto
        {
            public DayOfWeek DayOfWeek { get; set; } // 1 = Pazartesi
            public string StartTime { get; set; }    // "09:00"
            public string EndTime { get; set; }      // "17:00"
            public bool IsWorking { get; set; } = true;
        }
    }
}
