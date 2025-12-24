using Microsoft.AspNetCore.Mvc.ViewEngines;

namespace rezervasyonapi.Models
{
    public class Customer : ApplicationUser
    {
        // İlişkiler (1:Çok)
        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}
