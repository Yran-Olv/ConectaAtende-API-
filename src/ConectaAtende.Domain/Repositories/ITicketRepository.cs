using ConectaAtende.Domain.Entities;

namespace ConectaAtende.Domain.Repositories;

public interface ITicketRepository
{
    Task<Ticket?> GetByIdAsync(Guid id);
    Task<Ticket> AddAsync(Ticket ticket);
    Task<Ticket> UpdateAsync(Ticket ticket);
    Task<bool> ExistsAsync(Guid id);
    Task<IEnumerable<Ticket>> GetQueuedTicketsAsync();
}
