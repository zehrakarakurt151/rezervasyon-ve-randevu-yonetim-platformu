namespace rezervasyonapi.Models
{
    public class BlockedSlot : BaseEntity
    {
        public DateTimeOffset StartDateTime { get; set; }
        public DateTimeOffset EndDateTime { get; set; }
        public string Reason { get; set; }
        public BlockType Type { get; set; } // Mola mı, Kişisel İzin mi, Genel Tatil mi?

        // İlişki Anahtarları
        public int ServiceProviderId { get; set; }

        // İlişkiler (Çok:1)
        public ServiceProvider ServiceProvider { get; set; }
    }

    public enum BlockType
    {
        Break, // Gün içi kısa mola (Örn: Öğle yemeği)
        PersonalLeave, // Kişisel izin/randevu
        Holiday // Resmi tatil/İşletme tatili
    }
}
