using BCrypt.Net; // Şifreleme için
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens; // JWT için
using rezervasyonapi.DTOs;
using rezervasyonapi.Models;
using System.IdentityModel.Tokens.Jwt; // JWT için
using System.Security.Claims; // Claim'ler için
using System.Text; // Encoding için
using RezervasyonAPI.Data;
using RezervasyonAPI.Services;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context; // DbContext adınız neyse onu yazın
    private readonly IConfiguration _configuration;
    private readonly IEmailService _emailService;

    public AuthController(AppDbContext context, IConfiguration configuration, IEmailService emailService)
    {
        _context = context;
        _configuration = configuration;
        _emailService = emailService;
    }

    // POST: api/auth/register/customer
    [HttpPost("register/customer")]
    public async Task<IActionResult> RegisterCustomer([FromBody] CustomerRegisterDto request)
    {
        // 1. Email kontrolü: Bu email ile daha önce kayıt olunmuş mu?
        if (await _context.Customers.AnyAsync(u => u.Email == request.Email))
        {
            return BadRequest("Bu e-posta adresi zaten kullanımda.");
        }

        // 2. Şifre Hashleme (Güvenlik)
        string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

        // 3. Müşteri Nesnesini Oluşturma
        var newCustomer = new Customer
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber,
            PasswordHash = passwordHash, // Hashlenmiş şifreyi kaydediyoruz
            Role = UserRole.Customer,
            CreatedDate = DateTimeOffset.UtcNow,
            IsActive = true
        };

        _context.Customers.Add(newCustomer);
        await _context.SaveChangesAsync();

        // Hoşgeldin e-postası gönder (arka planda, hata olsa bile devam et)
        _ = _emailService.SendWelcomeEmailAsync(request.Email, request.FirstName, isProvider: false);

        return Ok(new { message = "Kayıt başarıyla oluşturuldu." });
    }

    // POST: api/auth/login
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto request)
    {
        // 1. Kullanıcıyı Bul (Hem Customer hem ServiceProvider tablosuna bakılabilir, şimdilik Customer)
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

        if (user == null)
        {
            return Unauthorized("E-posta veya şifre hatalı.");
        }

        // 2. Şifre Doğrulama
        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            return Unauthorized("E-posta veya şifre hatalı.");
        }

        // 3. Token Üretme
        string token = CreateToken(user);

        return Ok(new { token = token, userId = user.Id, role = user.Role.ToString() });
    }
    [HttpPost("register/provider")]
    public async Task<IActionResult> RegisterProvider([FromBody] ProviderRegisterDto request)
    {
        if (await _context.Users.AnyAsync(u => u.Email == request.Email))
        {
            return BadRequest("Bu e-posta adresi zaten kullanımda.");
        }

        string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

        // Dikkat: Burada 'ServiceProvider' modelini kullanıyoruz
        var newProvider = new rezervasyonapi.Models.ServiceProvider
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber,
            BusinessName = request.BusinessName,
            Address = request.Address,
            PasswordHash = passwordHash,
            Role = UserRole.ServiceProvider, // Rolü Sağlayıcı yaptık
            CreatedDate = DateTimeOffset.UtcNow,
            IsActive = true
        };

        _context.ServiceProviders.Add(newProvider);
        await _context.SaveChangesAsync();

        // Hoşgeldin e-postası gönder (arka planda, hata olsa bile devam et)
        _ = _emailService.SendWelcomeEmailAsync(request.Email, request.FirstName, isProvider: true);

        return Ok(new { message = "Hizmet Sağlayıcı kaydı başarıyla oluşturuldu." });
    }
    // JWT Token Üretici Yardımcı Metot
    private string CreateToken(ApplicationUser user)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role.ToString()) // Rol tabanlı yetkilendirme için önemli
        };

        // appsettings.json dosyasından gizli anahtarı alıyoruz
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("Jwt:Key").Value!));

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.Now.AddDays(1), // Token 1 gün geçerli
            signingCredentials: creds
        );

        var jwtHandler = new JwtSecurityTokenHandler();
        return jwtHandler.WriteToken(token);
    }
}