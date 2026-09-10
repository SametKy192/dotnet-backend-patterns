namespace ChainOfResponsibility.Api.Models;

public class TicketResult
{
    public Guid TicketId { get; set; }
    public bool IsProcessed { get; set; }
    public bool IsRejected { get; set; }
    public string? RejectionReason { get; set; }
    public string? AssignedTo { get; set; }
    public List<string> ProcessingLog { get; set; } = [];
    public DateTime ProcessedAt { get; set; } = DateTime.UtcNow;
}
