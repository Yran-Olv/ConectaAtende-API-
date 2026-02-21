namespace ConectaAtende.Domain.Entities;

public enum TicketStatus
{
    Created,
    Queued,
    InProgress,
    Completed,
    Cancelled
}

public enum TicketPriority
{
    Low,
    Medium,
    High
}

public class Ticket
{
    public Guid Id { get; set; }
    public Guid ContactId { get; set; }
    public string Description { get; set; } = string.Empty;
    public TicketStatus Status { get; set; }
    public TicketPriority Priority { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? QueuedAt { get; set; }
    public DateTime? DequeuedAt { get; set; }

    public Ticket()
    {
        Id = Guid.NewGuid();
        Status = TicketStatus.Created;
        Priority = TicketPriority.Medium;
        CreatedAt = DateTime.UtcNow;
    }

    public Ticket(Guid contactId, string description, TicketPriority priority = TicketPriority.Medium) : this()
    {
        ContactId = contactId;
        Description = description;
        Priority = priority;
    }

    public void Enqueue()
    {
        if (Status != TicketStatus.Created)
            throw new InvalidOperationException("Ticket já foi enfileirado ou está em outro estado");

        Status = TicketStatus.Queued;
        QueuedAt = DateTime.UtcNow;
    }

    public void Dequeue()
    {
        if (Status != TicketStatus.Queued)
            throw new InvalidOperationException("Ticket não está na fila");

        Status = TicketStatus.InProgress;
        DequeuedAt = DateTime.UtcNow;
    }

    public void UpdateStatus(TicketStatus newStatus)
    {
        Status = newStatus;
    }
}
