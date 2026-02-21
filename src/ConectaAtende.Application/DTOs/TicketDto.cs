namespace ConectaAtende.Application.DTOs;

public class TicketDto
{
    public Guid Id { get; set; }
    public Guid ContactId { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Priority { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? QueuedAt { get; set; }
    public DateTime? DequeuedAt { get; set; }
}

public class CreateTicketDto
{
    public Guid ContactId { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Priority { get; set; } = "Medium";
}
