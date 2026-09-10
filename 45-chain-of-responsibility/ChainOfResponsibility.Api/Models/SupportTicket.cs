namespace ChainOfResponsibility.Api.Models;

public class SupportTicket
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public TicketPriority Priority { get; set; }
    public TicketCategory Category { get; set; }
    public string RequestedBy { get; set; } = string.Empty;
    public string? ApiKey { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public List<string> ProcessingLog { get; set; } = [];
}

public enum TicketPriority { Low, Medium, High, Critical }
public enum TicketCategory { General, Technical, Billing, Security }
