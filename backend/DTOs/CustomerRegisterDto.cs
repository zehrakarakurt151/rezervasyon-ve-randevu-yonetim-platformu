namespace rezervasyonapi.DTOs
{
    
    public class CustomerRegisterDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; } // Kullanıcı ham şifreyi girer
        public string PhoneNumber { get; set; }
    }

    
    
}
