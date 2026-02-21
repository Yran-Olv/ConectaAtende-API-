using ConectaAtende.Domain.Entities;

namespace ConectaAtende.Domain.Services;

public interface ITriagePolicy
{
    Ticket? SelectNext(IEnumerable<Ticket> queuedTickets);
    string PolicyName { get; }
}
