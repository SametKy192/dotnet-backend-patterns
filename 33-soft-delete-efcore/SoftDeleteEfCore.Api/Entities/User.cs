namespace SoftDeleteEfCore.Api.Entities;

public class User : ISoftDeletable
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    // Soft Delete properties
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAtUtc { get; set; }
}
