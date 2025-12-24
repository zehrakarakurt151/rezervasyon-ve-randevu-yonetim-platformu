using System.Net;
using System.Net.Mail;

namespace RezervasyonAPI.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(string to, string subject, string body);
        Task SendWelcomeEmailAsync(string email, string firstName, bool isProvider);
        Task SendAppointmentCreatedEmailAsync(string customerEmail, string providerEmail, string serviceName, DateTime appointmentTime, string providerName);
        Task SendAppointmentCancelledEmailAsync(string email, string serviceName, DateTime appointmentTime, string reason, bool isCustomer);
    }

    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;
        private readonly bool _isEnabled;
        private readonly bool _simulateOnly;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
            _isEnabled = _configuration.GetValue<bool>("EmailSettings:Enabled");
            _simulateOnly = _configuration.GetValue<bool>("EmailSettings:SimulateOnly");
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            if (!_isEnabled)
            {
                _logger.LogInformation("[EMAIL-DISABLED] Would send to: {To}, Subject: {Subject}", to, subject);
                return;
            }

            // Simülasyon modu - gerçek e-posta göndermeden logla (jüri sunumları için ideal)
            if (_simulateOnly)
            {
                _logger.LogInformation("═══════════════════════════════════════════════════════════════");
                _logger.LogInformation("📧 E-POSTA SİMÜLASYONU (Gerçek gönderim yapılmadı)");
                _logger.LogInformation("═══════════════════════════════════════════════════════════════");
                _logger.LogInformation("📬 Alıcı: {To}", to);
                _logger.LogInformation("📌 Konu: {Subject}", subject);
                _logger.LogInformation("───────────────────────────────────────────────────────────────");
                _logger.LogInformation("📝 İçerik (HTML): {Body}", StripHtmlTags(body));
                _logger.LogInformation("═══════════════════════════════════════════════════════════════");
                await Task.CompletedTask;
                return;
            }

            try
            {
                var smtpHost = _configuration["EmailSettings:SmtpHost"];
                var smtpPort = _configuration.GetValue<int>("EmailSettings:SmtpPort");
                var username = _configuration["EmailSettings:Username"];
                var password = _configuration["EmailSettings:Password"];
                var fromEmail = _configuration["EmailSettings:FromEmail"];
                var fromName = _configuration["EmailSettings:FromName"];

                using var client = new SmtpClient(smtpHost, smtpPort)
                {
                    Credentials = new NetworkCredential(username, password),
                    EnableSsl = true
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(fromEmail!, fromName),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };
                mailMessage.To.Add(to);

                await client.SendMailAsync(mailMessage);
                _logger.LogInformation("✅ E-posta başarıyla gönderildi: {To}", to);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ E-posta gönderilemedi: {To}", to);
                // E-posta gönderiminde hata olsa bile uygulama çökmemeli
            }
        }

        private static string StripHtmlTags(string html)
        {
            // Basit HTML tag temizleme
            return System.Text.RegularExpressions.Regex.Replace(html, "<[^>]*>", " ")
                .Replace("&nbsp;", " ")
                .Replace("  ", " ")
                .Trim();
        }

        public async Task SendAppointmentCreatedEmailAsync(string customerEmail, string providerEmail, string serviceName, DateTime appointmentTime, string providerName)
        {
            var formattedDate = appointmentTime.ToString("dd.MM.yyyy HH:mm");

            // Müşteriye bildirim
            var customerSubject = "Randevunuz Oluşturuldu ✅";
            var customerBody = $@"
                <html>
                <body style='font-family: Arial, sans-serif; color: #333;'>
                    <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
                        <h2 style='color: #4CAF50;'>Randevunuz Başarıyla Oluşturuldu!</h2>
                        <p>Merhaba,</p>
                        <p>Aşağıdaki randevunuz başarıyla oluşturulmuştur:</p>
                        <div style='background: #f5f5f5; padding: 15px; border-radius: 8px; margin: 20px 0;'>
                            <p><strong>📅 Tarih:</strong> {formattedDate}</p>
                            <p><strong>💇 Hizmet:</strong> {serviceName}</p>
                            <p><strong>🏪 İşletme:</strong> {providerName}</p>
                        </div>
                        <p>Randevunuza zamanında gelmenizi rica ederiz.</p>
                        <p style='color: #888; font-size: 12px;'>Rezervasyon ve Randevu Yönetim Platformu</p>
                    </div>
                </body>
                </html>";

            await SendEmailAsync(customerEmail, customerSubject, customerBody);

            // Hizmet sağlayıcıya bildirim
            var providerSubject = "Yeni Randevu Bildirimi 📅";
            var providerBody = $@"
                <html>
                <body style='font-family: Arial, sans-serif; color: #333;'>
                    <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
                        <h2 style='color: #2196F3;'>Yeni Randevu Aldınız!</h2>
                        <p>Merhaba,</p>
                        <p>Yeni bir randevu talebi aldınız:</p>
                        <div style='background: #f5f5f5; padding: 15px; border-radius: 8px; margin: 20px 0;'>
                            <p><strong>📅 Tarih:</strong> {formattedDate}</p>
                            <p><strong>💇 Hizmet:</strong> {serviceName}</p>
                        </div>
                        <p style='color: #888; font-size: 12px;'>Rezervasyon ve Randevu Yönetim Platformu</p>
                    </div>
                </body>
                </html>";

            await SendEmailAsync(providerEmail, providerSubject, providerBody);
        }

        public async Task SendAppointmentCancelledEmailAsync(string email, string serviceName, DateTime appointmentTime, string reason, bool isCustomer)
        {
            var formattedDate = appointmentTime.ToString("dd.MM.yyyy HH:mm");
            var subject = "Randevu İptal Edildi ❌";

            var cancelledBy = isCustomer ? "Müşteri tarafından" : "Hizmet sağlayıcı tarafından";
            var body = $@"
                <html>
                <body style='font-family: Arial, sans-serif; color: #333;'>
                    <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
                        <h2 style='color: #f44336;'>Randevu İptal Edildi</h2>
                        <p>Aşağıdaki randevu iptal edilmiştir:</p>
                        <div style='background: #fff3f3; padding: 15px; border-radius: 8px; margin: 20px 0; border-left: 4px solid #f44336;'>
                            <p><strong>📅 Tarih:</strong> {formattedDate}</p>
                            <p><strong>💇 Hizmet:</strong> {serviceName}</p>
                            <p><strong>🚫 İptal Eden:</strong> {cancelledBy}</p>
                            {(string.IsNullOrEmpty(reason) ? "" : $"<p><strong>📝 Sebep:</strong> {reason}</p>")}
                        </div>
                        <p style='color: #888; font-size: 12px;'>Rezervasyon ve Randevu Yönetim Platformu</p>
                    </div>
                </body>
                </html>";

            await SendEmailAsync(email, subject, body);
        }

        public async Task SendWelcomeEmailAsync(string email, string firstName, bool isProvider)
        {
            var subject = "Randevu Sistemine Hoş Geldiniz! 🎉";
            var userType = isProvider ? "Hizmet Sağlayıcı" : "Müşteri";
            var additionalInfo = isProvider 
                ? "<p>Artık hizmetlerinizi ekleyebilir ve müşterilerinizden randevu alabilirsiniz.</p>"
                : "<p>Artık hizmet sağlayıcılarımızdan kolayca randevu alabilirsiniz.</p>";

            var body = $@"
                <html>
                <body style='font-family: Arial, sans-serif; color: #333;'>
                    <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
                        <h2 style='color: #673AB7;'>🎉 Hoş Geldiniz, {firstName}!</h2>
                        <p>Randevu ve Rezervasyon Sistemine başarıyla kayıt oldunuz.</p>
                        <div style='background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); padding: 20px; border-radius: 12px; margin: 20px 0; color: white;'>
                            <p style='margin: 0;'><strong>👤 Hesap Türü:</strong> {userType}</p>
                            <p style='margin: 10px 0 0 0;'><strong>📧 E-posta:</strong> {email}</p>
                        </div>
                        {additionalInfo}
                        <p>Herhangi bir sorunuz olursa bizimle iletişime geçmekten çekinmeyin.</p>
                        <hr style='border: none; border-top: 1px solid #eee; margin: 20px 0;'>
                        <p style='color: #888; font-size: 12px;'>Rezervasyon ve Randevu Yönetim Platformu</p>
                    </div>
                </body>
                </html>";

            await SendEmailAsync(email, subject, body);
        }
    }
}
