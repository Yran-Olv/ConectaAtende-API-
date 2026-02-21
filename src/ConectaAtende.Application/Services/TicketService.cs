using ConectaAtende.Application.DTOs;
using ConectaAtende.Domain.Entities;
using ConectaAtende.Domain.Repositories;
using ConectaAtende.Domain.Services;

namespace ConectaAtende.Application.Services;

public class TicketService
{
    private readonly ITicketRepository _ticketRepository;
    private readonly IContactRepository _contactRepository;
    private readonly ITriagePolicyService _triagePolicyService;

    public TicketService(
        ITicketRepository ticketRepository,
        IContactRepository contactRepository,
        ITriagePolicyService triagePolicyService)
    {
        _ticketRepository = ticketRepository;
        _contactRepository = contactRepository;
        _triagePolicyService = triagePolicyService;
    }

    public async Task<TicketDto> CreateAsync(CreateTicketDto dto)
    {
        var contactExists = await _contactRepository.ExistsAsync(dto.ContactId);
        if (!contactExists)
            throw new ArgumentException("Contato não encontrado");

        var priority = Enum.Parse<TicketPriority>(dto.Priority);
        var ticket = new Ticket(dto.ContactId, dto.Description, priority);
        var created = await _ticketRepository.AddAsync(ticket);
        return MapToDto(created);
    }

    public async Task<TicketDto?> GetByIdAsync(Guid id)
    {
        var ticket = await _ticketRepository.GetByIdAsync(id);
        return ticket == null ? null : MapToDto(ticket);
    }

    public async Task<TicketDto?> EnqueueAsync(Guid ticketId)
    {
        var ticket = await _ticketRepository.GetByIdAsync(ticketId);
        if (ticket == null)
            return null;

        ticket.Enqueue();
        var updated = await _ticketRepository.UpdateAsync(ticket);
        return MapToDto(updated);
    }

    public async Task<TicketDto?> GetNextAsync()
    {
        var queuedTickets = await _ticketRepository.GetQueuedTicketsAsync();
        var policy = _triagePolicyService.GetCurrentPolicy();
        var nextTicket = policy.SelectNext(queuedTickets);
        return nextTicket == null ? null : MapToDto(nextTicket);
    }

    public async Task<TicketDto?> DequeueAsync()
    {
        var queuedTickets = await _ticketRepository.GetQueuedTicketsAsync();
        var policy = _triagePolicyService.GetCurrentPolicy();
        var nextTicket = policy.SelectNext(queuedTickets);
        
        if (nextTicket == null)
            return null;

        nextTicket.Dequeue();
        var updated = await _ticketRepository.UpdateAsync(nextTicket);
        return MapToDto(updated);
    }

    private static TicketDto MapToDto(Ticket ticket)
    {
        return new TicketDto
        {
            Id = ticket.Id,
            ContactId = ticket.ContactId,
            Description = ticket.Description,
            Status = ticket.Status.ToString(),
            Priority = ticket.Priority.ToString(),
            CreatedAt = ticket.CreatedAt,
            QueuedAt = ticket.QueuedAt,
            DequeuedAt = ticket.DequeuedAt
        };
    }
}
