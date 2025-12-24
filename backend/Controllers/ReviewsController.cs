using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using rezervasyonapi.DTOs;
using rezervasyonapi.Models;
using RezervasyonAPI.Data;
using System.Security.Claims;

namespace rezervasyonapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ReviewsController(AppDbContext context)
        {
            _context = context;
        }

        // POST: api/reviews
        [Authorize(Roles = "Customer")]
        [HttpPost]
        public async Task<IActionResult> CreateReview([FromBody] CreateReviewDto request)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            // 1. Randevuyu Kontrol Et
            var appointment = await _context.Appointments
                .Include(a => a.Review) // Zaten yorum var mı diye bakmak için
                .FirstOrDefaultAsync(a => a.Id == request.AppointmentId);

            if (appointment == null) return NotFound("Randevu bulunamadı.");

            // 2. Güvenlik Kontrolleri
            if (appointment.CustomerId != userId)
                return Unauthorized("Bu randevuya yorum yapma yetkiniz yok.");

            if (appointment.Status != AppointmentStatus.Completed)
                return BadRequest("Henüz tamamlanmamış bir hizmete yorum yapamazsınız.");

            if (appointment.Review != null)
                return BadRequest("Bu randevu için zaten yorum yaptınız.");

            if (request.Rating < 1 || request.Rating > 5)
                return BadRequest("Puan 1 ile 5 arasında olmalıdır.");

            // 3. Yorumu Kaydet
            var newReview = new Review
            {
                AppointmentId = request.AppointmentId,
                CustomerId = userId,
                ServiceProviderId = appointment.ServiceProviderId,
                Rating = request.Rating,
                Comment = request.Comment,
                CreatedDate = DateTimeOffset.UtcNow,
                IsActive = true
            };

            _context.Reviews.Add(newReview);
            await _context.SaveChangesAsync();

            // 4. DÜKKANIN ORTALAMA PUANINI GÜNCELLE (Otomatik Hesaplama)
            var providerId = appointment.ServiceProviderId;

            // O dükkana ait tüm puanların ortalamasını al
            double newAverage = await _context.Reviews
                .Where(r => r.ServiceProviderId == providerId)
                .AverageAsync(r => r.Rating);

            var provider = await _context.ServiceProviders.FindAsync(providerId);
            provider.AverageRating = Math.Round(newAverage, 1); // Virgülden sonra 1 basamak (Örn: 4.5)

            await _context.SaveChangesAsync();

            return Ok(new { message = "Yorumunuz kaydedildi ve dükkan puanı güncellendi." });
        }

        // GET: api/reviews/provider/5 (Dükkanın Yorumlarını Listele)
        [HttpGet("provider/{providerId}")]
        public async Task<IActionResult> GetProviderReviews(int providerId)
        {
            var reviews = await _context.Reviews
                .Include(r => r.Customer)
                .Where(r => r.ServiceProviderId == providerId)
                .OrderByDescending(r => r.CreatedDate)
                .Select(r => new
                {
                    r.Id,
                    r.Rating,
                    r.Comment,
                    CustomerName = r.Customer.FirstName, // Sadece adını göster, soyadı gizli kalsın
                    Date = r.CreatedDate
                })
                .ToListAsync();

            return Ok(reviews);
        }
    }
}