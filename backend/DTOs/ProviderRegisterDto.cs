namespace rezervasyonapi.DTOs
{
    public class ProviderRegisterDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string PhoneNumber { get; set; }
        public string BusinessName { get; set; } // İşletme Adı
        public string Address { get; set; }
    }
}
