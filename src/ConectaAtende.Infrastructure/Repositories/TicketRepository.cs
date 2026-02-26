using ConectaAtende.Domain.Entities;
using ConectaAtende.Domain.Repositories;
using ConectaAtende.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ConectaAtende.Infrastructure.Repositories;

public class TicketRepository : ITicketRepository
{
    private readonly ConectaAtendeDbContext _context;

    public TicketRepository(ConectaAtendeDbContext context)
    {
        _context = context;
    }

    public async Task<Ticket?> GetByIdAsync(Guid id)
    {
        return await _context.Tickets.FindAsync(id);
    }

    public async Task<Ticket> AddAsync(Ticket ticket)
    {
        _context.Tickets.Add(ticket);
        await _context.SaveChangesAsync();
        return ticket;
    }

    public async Task<Ticket> UpdateAsync(Ticket ticket)
    {
        var existing = await _context.Tickets.FindAsync(ticket.Id);
        if (existing == null)
            throw new KeyNotFoundException("Ticket não encontrado");

        existing.Status = ticket.Status;
        existing.Priority = ticket.Priority;
        existing.Description = ticket.Description;
        existing.QueuedAt = ticket.QueuedAt;
        existing.DequeuedAt = ticket.DequeuedAt;

        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.Tickets.AnyAsync(t => t.Id == id);
    }

    public async Task<IEnumerable<Ticket>> GetQueuedTicketsAsync()
    {
        return await _context.Tickets
            .Where(t => t.Status == TicketStatus.Queued)
            .ToListAsync();
    }
}
