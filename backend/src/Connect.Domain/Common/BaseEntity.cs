namespace Connect.Domain.Common;

/// <summary>
/// Base type for all persisted entities.
/// Uses a sequential GUID (v7) primary key: globally unique like a GUID, but
/// time-ordered so it stays index-friendly on SQL Server (avoids page splits).
/// </summary>
public abstract class BaseEntity
{
    public Guid Id { get; protected set; } = Guid.CreateVersion7();

    public DateTime CreatedAtUtc { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }
}
