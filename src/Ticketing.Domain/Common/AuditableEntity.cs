namespace Ticketing.Domain.Common
{
    public abstract class AuditableEntity : Entity
    {
        public Guid? CreatedBy { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public Guid? ModifiedBy { get; private set; }
        public DateTime modifiedAt { get; private set; }
        public bool IsActive { get; private set; } = true;

        public void MarkAsCreated(Guid? userId, DateTime utcNow)
        {
            CreatedBy = userId;
            CreatedAt = utcNow;
        }

        public void MarkAsModified(Guid? userId, DateTime utcNow)
        {
            ModifiedBy = userId;
            modifiedAt = utcNow;
        }

        public void Deactivate() => IsActive = false;
        public void Activate() => IsActive = true;
    }
}
