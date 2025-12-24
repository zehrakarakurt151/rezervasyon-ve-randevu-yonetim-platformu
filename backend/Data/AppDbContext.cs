using Microsoft.EntityFrameworkCore;
using rezervasyonapi.Models;
 
namespace RezervasyonAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // --- DbSets (Veritabanı Tabloları) ---

        // Kullanıcılar (TPH - Table Per Hierarchy yaklaşımıyla tek tabloda tutulur)
        public DbSet<ApplicationUser> Users { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<rezervasyonapi.Models.ServiceProvider> ServiceProviders { get; set; }

        // İş Modelleri
        public DbSet<Service> Services { get; set; }
        public DbSet<WorkSchedule> WorkSchedules { get; set; }
        public DbSet<BlockedSlot> BlockedSlots { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<Review> Reviews { get; set; }

        // --- Konfigürasyonlar ---
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 1. Kullanıcı Kalıtım Ayarı (Discriminator)
            // EF Core, abstract class olduğu için otomatik olarak tek tablo (TPH) oluşturur.
            // "Discriminator" kolonu ile Müşteri mi Sağlayıcı mı olduğunu ayırır.

            // 2. Fiyat Alanları için Hassasiyet (Decimal Precision)
            // C#'taki decimal tipi için SQL'de boyut belirtmezsek hata/uyarı verir.
            modelBuilder.Entity<Service>()
                .Property(s => s.Price)
                .HasPrecision(18, 2); // Toplam 18 basamak, virgülden sonra 2 basamak

            modelBuilder.Entity<Appointment>()
                .Property(a => a.TotalPrice)
                .HasPrecision(18, 2);

            // 3. İlişki ve Silme Davranışları (Delete Behavior) - SQL Server için NoAction
            // SQL Server'da birden fazla cascade yolu olduğunda hata veriyor, bu yüzden NoAction kullanıyoruz.

            modelBuilder.Entity<rezervasyonapi.Models.ServiceProvider>()
                .HasMany(sp => sp.Services)
                .WithOne(s => s.ServiceProvider)
                .HasForeignKey(s => s.ServiceProviderId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.ServiceProvider)
                .WithMany(sp => sp.Appointments)
                .HasForeignKey(a => a.ServiceProviderId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Customer)
                .WithMany(c => c.Appointments)
                .HasForeignKey(a => a.CustomerId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Service)
                .WithMany(s => s.Appointments)
                .HasForeignKey(a => a.ServiceId)
                .OnDelete(DeleteBehavior.NoAction);

            // Review ilişkileri
            modelBuilder.Entity<Review>()
                .HasOne(r => r.Appointment)
                .WithOne(a => a.Review)
                .HasForeignKey<Review>(r => r.AppointmentId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Review>()
                .HasOne(r => r.Customer)
                .WithMany(c => c.Reviews)
                .HasForeignKey(r => r.CustomerId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Review>()
                .HasOne(r => r.ServiceProvider)
                .WithMany(sp => sp.ReviewsReceived)
                .HasForeignKey(r => r.ServiceProviderId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}