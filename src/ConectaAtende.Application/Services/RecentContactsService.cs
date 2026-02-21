using ConectaAtende.Application.DTOs;
using ConectaAtende.Domain.Repositories;

namespace ConectaAtende.Application.Services;

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
