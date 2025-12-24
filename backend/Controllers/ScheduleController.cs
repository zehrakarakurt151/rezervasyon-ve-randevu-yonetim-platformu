using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using rezervasyonapi.DTOs.RezervasyonAPI.DTOs;
using rezervasyonapi.Models;
using RezervasyonAPI.Data;

using System.Security.Claims;

namespace RezervasyonAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScheduleController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ScheduleController(AppDbContext context)
        {
            _context = context;
        }

        // POST: api/schedule/set-schedule
        // Sağlayıcı haftalık programını buraya gönderir
        [Authorize]
        [HttpPost("set-schedule")]
        public async Task<IActionResult> SetSchedule([FromBody] List<WorkScheduleDto> schedules)
        {
            // Token'dan giriş yapan kullanıcının ID'sini al
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            // Önceki kayıtlı programı temizle (Güncelleme mantığı yerine sil-ekle yapıyoruz)
            var oldSchedules = _context.WorkSchedules.Where(w => w.ServiceProviderId == userId);
            _context.WorkSchedules.RemoveRange(oldSchedules);

            // Yeni saatleri ekle
            foreach (var item in schedules)
            {
                var schedule = new WorkSchedule
                {
                    ServiceProviderId = userId,
                    DayOfWeek = item.DayOfWeek,
                    StartTime = TimeSpan.Parse(item.StartTime),
                    EndTime = TimeSpan.Parse(item.EndTime),
                    IsWorking = item.IsWorking
                };
                _context.WorkSchedules.Add(schedule);
            }

            await _context.SaveChangesAsync();
            return Ok(new { message = "Çalışma saatleri başarıyla güncellendi." });
        }
    }
}