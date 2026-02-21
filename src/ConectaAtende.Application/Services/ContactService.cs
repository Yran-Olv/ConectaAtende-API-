using ConectaAtende.Application.DTOs;
using ConectaAtende.Domain.Entities;
using ConectaAtende.Domain.Repositories;

namespace ConectaAtende.Application.Services;

public class ContactService
{
    private readonly IContactRepository _repository;

    public ContactService(IContactRepository repository)
    {
        _repository = repository;
    }

    public async Task<ContactDto?> GetByIdAsync(Guid id)
    {
        var contact = await _repository.GetByIdAsync(id);
        return contact == null ? null : MapToDto(contact);
    }

    public async Task<PaginatedResult<ContactDto>> GetAllAsync(int page, int pageSize)
    {
        var contacts = await _repository.GetAllAsync(page, pageSize);
        var allContacts = await _repository.GetAllAsync(1, int.MaxValue);
        
        return new PaginatedResult<ContactDto>
        {
            Items = contacts.Select(MapToDto).ToList(),
            Page = page,
            PageSize = pageSize,
            TotalCount = allContacts.Count()
        };
    }

    public async Task<IEnumerable<ContactDto>> SearchByNameAsync(string name)
    {
        if (string.IsNullOrWhiteSpace(name) || name.Length < 3)
            return Enumerable.Empty<ContactDto>();

        var contacts = await _repository.SearchByNameAsync(name);
        return contacts.Select(MapToDto);
    }

    public async Task<IEnumerable<ContactDto>> SearchByPhoneAsync(string phone)
    {
        if (string.IsNullOrWhiteSpace(phone))
            return Enumerable.Empty<ContactDto>();

        var contacts = await _repository.SearchByPhoneAsync(phone);
        return contacts.Select(MapToDto);
    }

    public async Task<ContactDto> CreateAsync(CreateContactDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
            throw new ArgumentException("Nome é obrigatório");

        var contact = new Contact(dto.Name, dto.Phones);
        var created = await _repository.AddAsync(contact);
        return MapToDto(created);
    }

    public async Task<ContactDto?> UpdateAsync(Guid id, UpdateContactDto dto)
    {
        var contact = await _repository.GetByIdAsync(id);
        if (contact == null)
            return null;

        if (string.IsNullOrWhiteSpace(dto.Name))
            throw new ArgumentException("Nome é obrigatório");

        contact.Update(dto.Name, dto.Phones);
        var updated = await _repository.UpdateAsync(contact);
        return MapToDto(updated);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        return await _repository.DeleteAsync(id);
    }

    private static ContactDto MapToDto(Contact contact)
    {
        return new ContactDto
        {
            Id = contact.Id,
            Name = contact.Name,
            Phones = contact.Phones,
            CreatedAt = contact.CreatedAt,
            UpdatedAt = contact.UpdatedAt
        };
    }
}
