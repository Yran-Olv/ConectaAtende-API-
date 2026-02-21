using ConectaAtende.Domain.Entities;

namespace ConectaAtende.Domain.Repositories;

public interface IContactRepository
{
    Task<Contact?> GetByIdAsync(Guid id);
    Task<IEnumerable<Contact>> GetAllAsync(int page, int pageSize);
    Task<IEnumerable<Contact>> SearchByNameAsync(string name);
    Task<IEnumerable<Contact>> SearchByPhoneAsync(string phone);
    Task<Contact> AddAsync(Contact contact);
    Task<Contact> UpdateAsync(Contact contact);
    Task<bool> DeleteAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
}
