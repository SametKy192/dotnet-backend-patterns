namespace InboxPattern.Infrastructure.Entities;

public class InboxMessage
{
    public Guid Id { get; set; }
    public string EventType { get; set; } = string.Empty;
    public string Payload { get; set; } = string.Empty;
    public DateTime OccurredOn { get; set; }
    public DateTime? ProcessedAt { get; set; }
    public string? ErrorMessage { get; set; }

    /// <summary>Indicates whether the inbox message has been successfully processed.</summary>
    public bool IsProcessed => ProcessedAt.HasValue && string.IsNullOrEmpty(ErrorMessage);
}
