
namespace rezervasyonapi.Models
{
    public abstract class ApplicationUser : BaseEntity
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string PasswordHash { get; set; } // Identity kullanmıyorsanız bu alan gerekir
        public UserRole Role { get; set; } // Müşteri, Sağlayıcı veya Admin rolü

        // İlişkiler: (Diğer modellerde tanımlanacak)
    }

    public enum UserRole
    {
        Customer = 0,
        ServiceProvider = 1,
        Admin = 2
    }
}
