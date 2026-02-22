using ConectaAtende.Application.DTOs;
using ConectaAtende.Application.Services;
using ConectaAtende.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;

namespace ConectaAtende.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ContactsController : ControllerBase
{
    private readonly ContactService _contactService;
    private readonly UndoService _undoService;
    private readonly RecentContactsService _recentContactsService;

    public ContactsController(
        ContactService contactService,
        UndoService undoService,
        RecentContactsService recentContactsService)
    {
        _contactService = contactService;
        _undoService = undoService;
        _recentContactsService = recentContactsService;
    }

    [HttpPost]
    public async Task<ActionResult<ContactDto>> Create([FromBody] CreateContactDto dto)
    {
        try
        {
            var contact = await _contactService.CreateAsync(dto);
            _undoService.RecordCreate(contact.ToDomain());
            return CreatedAtAction(nameof(GetById), new { id = contact.Id }, contact);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ContactDto>> GetById(Guid id)
    {
        var contact = await _contactService.GetByIdAsync(id);
        if (contact == null)
            return NotFound();

        _recentContactsService.AddRecent(id);
        return Ok(contact);
    }

    [HttpGet]
    public async Task<ActionResult<PaginatedResult<ContactDto>>> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var result = await _contactService.GetAllAsync(page, pageSize);
        return Ok(result);
    }

    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<ContactDto>>> Search(
        [FromQuery] string? name = null,
        [FromQuery] string? phone = null)
    {
        if (!string.IsNullOrWhiteSpace(name))
        {
            var results = await _contactService.SearchByNameAsync(name);
            return Ok(results);
        }

        if (!string.IsNullOrWhiteSpace(phone))
        {
            var results = await _contactService.SearchByPhoneAsync(phone);
            return Ok(results);
        }

        return BadRequest("Informe 'name' ou 'phone' para busca");
    }

    [HttpGet("recent")]
    public async Task<ActionResult<IEnumerable<ContactDto>>> GetRecent([FromQuery] int limit = 10)
    {
        var recent = await _recentContactsService.GetRecentAsync(limit);
        return Ok(recent);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ContactDto>> Update(Guid id, [FromBody] UpdateContactDto dto)
    {
        var existing = await _contactService.GetByIdAsync(id);
        if (existing == null)
            return NotFound();

        _undoService.RecordUpdate(existing.ToDomain());
        var updated = await _contactService.UpdateAsync(id, dto);
        if (updated == null)
            return NotFound();

        return Ok(updated);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var existing = await _contactService.GetByIdAsync(id);
        if (existing == null)
            return NotFound();

        _undoService.RecordDelete(existing.ToDomain());
        _recentContactsService.RemoveRecent(id);
        var deleted = await _contactService.DeleteAsync(id);

        if (!deleted)
            return NotFound();

        return NoContent();
    }

    [HttpPost("undo")]
    public async Task<IActionResult> Undo()
    {
        var success = await _undoService.UndoAsync();
        if (!success)
            return BadRequest("Não há operação para desfazer");

        return Ok(new { message = "Operação desfeita com sucesso" });
    }
}

public static class ContactDtoExtensions
{
    public static ConectaAtende.Domain.Entities.Contact ToDomain(this ContactDto dto)
    {
        return new ConectaAtende.Domain.Entities.Contact
        {
            Id = dto.Id,
            Name = dto.Name,
            Phones = dto.Phones,
            CreatedAt = dto.CreatedAt,
            UpdatedAt = dto.UpdatedAt
        };
    }
}
