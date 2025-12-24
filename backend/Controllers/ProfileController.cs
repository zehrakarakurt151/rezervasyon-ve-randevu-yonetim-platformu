using BCrypt.Net; // Şifreleme için
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using rezervasyonapi.DTOs;
using RezervasyonAPI.Data;
using System.Security.Claims;

namespace rezervasyonapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Sadece giriş yapmışlar kullanabilir
    public class ProfileController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProfileController(AppDbContext context)
        {
            _context = context;
        }

        // 1. BİLGİLERİ GÜNCELLE
        // PUT: api/profile/update-info
        [HttpPut("update-info")]
        public async Task<IActionResult> UpdateInfo([FromBody] UpdateProfileDto request)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            // Kullanıcıyı Users tablosundan bul (Hem Customer hem Provider olabilir)
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return NotFound("Kullanıcı bulunamadı.");

            user.FirstName = request.FirstName;
            user.LastName = request.LastName;
            user.PhoneNumber = request.PhoneNumber;
            user.UpdatedDate = DateTimeOffset.UtcNow;

            await _context.SaveChangesAsync();
            return Ok(new { message = "Profil bilgileri güncellendi." });
        }

        // 2. ŞİFRE DEĞİŞTİR
        // PUT: api/profile/change-password
        [HttpPut("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto request)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var user = await _context.Users.FindAsync(userId);

            // 1. Eski şifre doğru mu?
            if (!BCrypt.Net.BCrypt.Verify(request.OldPassword, user.PasswordHash))
            {
                return BadRequest("Mevcut şifreniz hatalı.");
            }

            // 2. Yeni şifreyi hashle ve kaydet
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Şifreniz başarıyla değiştirildi." });
        }
    }
}