using System.Globalization;
using System.Text;
using ConectaAtende.Domain.Entities;
using ConectaAtende.Domain.Repositories;

namespace ConectaAtende.Infrastructure.Repositories;

public class InMemoryContactRepository : IContactRepository
{
    private readonly Dictionary<Guid, Contact> _contacts = new();
    private readonly Dictionary<string, HashSet<Guid>> _nameIndex = new();
    private readonly Dictionary<string, HashSet<Guid>> _phoneIndex = new();

    public Task<Contact?> GetByIdAsync(Guid id)
    {
        _contacts.TryGetValue(id, out var contact);
        return Task.FromResult(contact);
    }

    public Task<IEnumerable<Contact>> GetAllAsync(int page, int pageSize)
    {
        var contacts = _contacts.Values
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return Task.FromResult<IEnumerable<Contact>>(contacts);
    }

    public Task<IEnumerable<Contact>> SearchByNameAsync(string name)
    {
        if (string.IsNullOrWhiteSpace(name) || name.Length < 3)
            return Task.FromResult(Enumerable.Empty<Contact>());

        var normalizedName = NormalizeString(name);
        var results = new HashSet<Contact>();

        foreach (var kvp in _nameIndex)
        {
            if (kvp.Key.Contains(normalizedName))
            {
                foreach (var contactId in kvp.Value)
                {
                    if (_contacts.TryGetValue(contactId, out var contact))
                    {
                        results.Add(contact);
                    }
                }
            }
        }

        return Task.FromResult<IEnumerable<Contact>>(results);
    }

    public Task<IEnumerable<Contact>> SearchByPhoneAsync(string phone)
    {
        if (string.IsNullOrWhiteSpace(phone))
            return Task.FromResult(Enumerable.Empty<Contact>());

        var normalizedPhone = new string(phone.Where(char.IsDigit).ToArray());
        var results = new HashSet<Contact>();

        if (_phoneIndex.TryGetValue(normalizedPhone, out var contactIds))
        {
            foreach (var contactId in contactIds)
            {
                if (_contacts.TryGetValue(contactId, out var contact))
                {
                    results.Add(contact);
                }
            }
        }

        return Task.FromResult<IEnumerable<Contact>>(results);
    }

    public Task<Contact> AddAsync(Contact contact)
    {
        _contacts[contact.Id] = contact;
        UpdateIndexes(contact);
        return Task.FromResult(contact);
    }

    public Task<Contact> UpdateAsync(Contact contact)
    {
        if (!_contacts.ContainsKey(contact.Id))
            throw new KeyNotFoundException("Contato não encontrado");

        RemoveFromIndexes(contact.Id);
        _contacts[contact.Id] = contact;
        UpdateIndexes(contact);

        return Task.FromResult(contact);
    }

    public Task<bool> DeleteAsync(Guid id)
    {
        if (!_contacts.ContainsKey(id))
            return Task.FromResult(false);

        RemoveFromIndexes(id);
        _contacts.Remove(id);
        return Task.FromResult(true);
    }

    public Task<bool> ExistsAsync(Guid id)
    {
        return Task.FromResult(_contacts.ContainsKey(id));
    }

    private void UpdateIndexes(Contact contact)
    {
        var normalizedName = NormalizeString(contact.Name);
        if (!_nameIndex.ContainsKey(normalizedName))
        {
            _nameIndex[normalizedName] = new HashSet<Guid>();
        }
        _nameIndex[normalizedName].Add(contact.Id);

        foreach (var phone in contact.Phones)
        {
            var normalizedPhone = new string(phone.Where(char.IsDigit).ToArray());
            if (!_phoneIndex.ContainsKey(normalizedPhone))
            {
                _phoneIndex[normalizedPhone] = new HashSet<Guid>();
            }
            _phoneIndex[normalizedPhone].Add(contact.Id);
        }
    }

    private void RemoveFromIndexes(Guid contactId)
    {
        if (!_contacts.TryGetValue(contactId, out var contact))
            return;

        var normalizedName = NormalizeString(contact.Name);
        if (_nameIndex.TryGetValue(normalizedName, out var nameSet))
        {
            nameSet.Remove(contactId);
            if (nameSet.Count == 0)
            {
                _nameIndex.Remove(normalizedName);
            }
        }

        foreach (var phone in contact.Phones)
        {
            var normalizedPhone = new string(phone.Where(char.IsDigit).ToArray());
            if (_phoneIndex.TryGetValue(normalizedPhone, out var phoneSet))
            {
                phoneSet.Remove(contactId);
                if (phoneSet.Count == 0)
                {
                    _phoneIndex.Remove(normalizedPhone);
                }
            }
        }
    }

    private static string NormalizeString(string input)
    {
        var normalized = input.Normalize(NormalizationForm.FormD);
        var stringBuilder = new StringBuilder();

        foreach (var c in normalized)
        {
            var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
            if (unicodeCategory != UnicodeCategory.NonSpacingMark)
            {
                stringBuilder.Append(c);
            }
        }

        return stringBuilder.ToString().ToLowerInvariant();
    }
}
