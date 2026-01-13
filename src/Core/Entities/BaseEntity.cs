namespace Server.Core.Entities;

/// <summary>
/// Base entity class for all domain models
/// </summary>
public abstract class BaseEntity
{
    public uint Id { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public virtual void UpdateTimestamp()
    {
        UpdatedAt = DateTime.UtcNow;
    }
}