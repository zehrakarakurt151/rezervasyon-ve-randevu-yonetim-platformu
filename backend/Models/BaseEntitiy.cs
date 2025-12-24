namespace rezervasyonapi.Models
{
    public abstract class BaseEntity
    {
        public int Id { get; set; }
        public DateTimeOffset CreatedDate { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset? UpdatedDate { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
