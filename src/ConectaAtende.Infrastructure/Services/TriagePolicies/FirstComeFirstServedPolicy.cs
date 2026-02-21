using ConectaAtende.Domain.Entities;
using ConectaAtende.Domain.Services;

namespace ConectaAtende.Infrastructure.Services.TriagePolicies;

public class FirstComeFirstServedPolicy : ITriagePolicy
{
    public string PolicyName => "FirstComeFirstServed";

    public Ticket? SelectNext(IEnumerable<Ticket> queuedTickets)
    {
        return queuedTickets
            .OrderBy(t => t.QueuedAt ?? t.CreatedAt)
            .FirstOrDefault();
    }
}
