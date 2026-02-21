using ConectaAtende.Application.DTOs;
using ConectaAtende.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace ConectaAtende.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TicketsController : ControllerBase
{
    private readonly TicketService _ticketService;

    public TicketsController(TicketService ticketService)
    {
        _ticketService = ticketService;
    }

    [HttpPost]
    public async Task<ActionResult<TicketDto>> Create([FromBody] CreateTicketDto dto)
    {
        try
        {
            var ticket = await _ticketService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = ticket.Id }, ticket);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TicketDto>> GetById(Guid id)
    {
        var ticket = await _ticketService.GetByIdAsync(id);
        if (ticket == null)
            return NotFound();

        return Ok(ticket);
    }

    [HttpPost("enqueue/{ticketId}")]
    public async Task<ActionResult<TicketDto>> Enqueue(Guid ticketId)
    {
        var ticket = await _ticketService.EnqueueAsync(ticketId);
        if (ticket == null)
            return NotFound();

        return Ok(ticket);
    }

    [HttpGet("next")]
    public async Task<ActionResult<TicketDto>> GetNext()
    {
        var ticket = await _ticketService.GetNextAsync();
        if (ticket == null)
            return NotFound();

        return Ok(ticket);
    }

    [HttpPost("dequeue")]
    public async Task<ActionResult<TicketDto>> Dequeue()
    {
        var ticket = await _ticketService.DequeueAsync();
        if (ticket == null)
            return NotFound();

        return Ok(ticket);
    }
}
