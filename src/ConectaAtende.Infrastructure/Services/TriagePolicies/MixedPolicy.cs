using ConectaAtende.Domain.Entities;
using ConectaAtende.Domain.Services;

namespace ConectaAtende.Infrastructure.Services.TriagePolicies;

public class MixedPolicy : ITriagePolicy
{
    public string PolicyName => "Mixed";

    public Ticket? SelectNext(IEnumerable<Ticket> queuedTickets)
    {
        var priorityOrder = new Dictionary<TicketPriority, int>
        {
            { TicketPriority.High, 1 },
            { TicketPriority.Medium, 2 },
            { TicketPriority.Low, 3 }
        };

        var highPriorityTickets = queuedTickets
            .Where(t => t.Priority == TicketPriority.High)
            .OrderBy(t => t.QueuedAt ?? t.CreatedAt)
            .ToList();

        if (highPriorityTickets.Any())
        {
            return highPriorityTickets.First();
        }

        var mediumPriorityTickets = queuedTickets
            .Where(t => t.Priority == TicketPriority.Medium)
            .OrderBy(t => t.QueuedAt ?? t.CreatedAt)
            .ToList();

        if (mediumPriorityTickets.Any())
        {
            return mediumPriorityTickets.First();
        }

        return queuedTickets
            .OrderBy(t => t.QueuedAt ?? t.CreatedAt)
            .FirstOrDefault();
    }
}
