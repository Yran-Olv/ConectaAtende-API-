using ConectaAtende.Domain.Entities;
using ConectaAtende.Domain.Services;

namespace ConectaAtende.Infrastructure.Services.TriagePolicies;

public class PriorityPolicy : ITriagePolicy
{
    public string PolicyName => "Priority";

    public Ticket? SelectNext(IEnumerable<Ticket> queuedTickets)
    {
        var priorityOrder = new Dictionary<TicketPriority, int>
        {
            { TicketPriority.High, 1 },
            { TicketPriority.Medium, 2 },
            { TicketPriority.Low, 3 }
        };

        return queuedTickets
            .OrderBy(t => priorityOrder[t.Priority])
            .ThenBy(t => t.QueuedAt ?? t.CreatedAt)
            .FirstOrDefault();
    }
}
