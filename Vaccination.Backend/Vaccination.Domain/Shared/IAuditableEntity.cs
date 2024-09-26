namespace Vaccination.Domain.Shared
{
    public interface IAuditableEntity
    {
        Guid CreatedBy { get; set; }
        Guid? ModifiedBy { get; set; }
        DateTime CreatedOnUtc { get; set; }
        DateTime? ModifiedOnUtc { get; set; }
        bool IsDeleted { get; set; }
    }
}