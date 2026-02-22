using ConectaAtende.Application.DTOs;
using ConectaAtende.Domain.Repositories;

namespace ConectaAtende.Infrastructure.Services;

/// <summary>
/// Implementação da lista de contatos recentes usando LinkedList.
/// Responsabilidade na camada Infrastructure, conforme Clean Architecture.
/// LinkedList permite inserção/remoção O(1) no início e no meio (com nó).
/// </summary>
public class RecentContactsService
{
    private readonly IContactRepository _repository;
    private readonly LinkedList<Guid> _recentIds = new();
    private readonly int _maxCapacity;

    public RecentContactsService(IContactRepository repository, int maxCapacity = 10)
    {
        _repository = repository;
        _maxCapacity = maxCapacity;
    }

    public void AddRecent(Guid contactId)
    {
        var existingNode = _recentIds.Find(contactId);
        if (existingNode != null)
        {
            _recentIds.Remove(existingNode);
        }

        _recentIds.AddFirst(contactId);

        while (_recentIds.Count > _maxCapacity)
        {
            _recentIds.RemoveLast();
        }
    }

    public void RemoveRecent(Guid contactId)
    {
        var existingNode = _recentIds.Find(contactId);
        if (existingNode != null)
        {
            _recentIds.Remove(existingNode);
        }
    }

    public async Task<IEnumerable<ContactDto>> GetRecentAsync(int limit)
    {
        var ids = _recentIds.Take(limit).ToList();
        var contacts = new List<ContactDto>();

        foreach (var id in ids)
        {
            var contact = await _repository.GetByIdAsync(id);
            if (contact != null)
            {
                contacts.Add(new ContactDto
                {
                    Id = contact.Id,
                    Name = contact.Name,
                    Phones = contact.Phones,
                    CreatedAt = contact.CreatedAt,
                    UpdatedAt = contact.UpdatedAt
                });
            }
        }

        return contacts;
    }
}
