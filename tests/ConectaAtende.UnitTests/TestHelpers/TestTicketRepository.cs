using ConectaAtende.Domain.Entities;
using ConectaAtende.Domain.Repositories;

namespace ConectaAtende.UnitTests.TestHelpers;

/// <summary>
/// Repositório em memória APENAS para testes unitários.
/// Em produção, usar TicketRepository com EF Core.
/// </summary>
public class TestTicketRepository : ITicketRepository
{
    private readonly Dictionary<Guid, Ticket> _tickets = new();

    public Task<Ticket?> GetByIdAsync(Guid id)
    {
        _tickets.TryGetValue(id, out var ticket);
        return Task.FromResult(ticket);
    }

    public Task<Ticket> AddAsync(Ticket ticket)
    {
        _tickets[ticket.Id] = ticket;
        return Task.FromResult(ticket);
    }

    public Task<Ticket> UpdateAsync(Ticket ticket)
    {
        if (!_tickets.ContainsKey(ticket.Id))
            throw new KeyNotFoundException("Ticket não encontrado");

        _tickets[ticket.Id] = ticket;
        return Task.FromResult(ticket);
    }

    public Task<bool> ExistsAsync(Guid id)
    {
        return Task.FromResult(_tickets.ContainsKey(id));
    }

    public Task<IEnumerable<Ticket>> GetQueuedTicketsAsync()
    {
        var queued = _tickets.Values
            .Where(t => t.Status == TicketStatus.Queued)
            .ToList();

        return Task.FromResult<IEnumerable<Ticket>>(queued);
    }
}
