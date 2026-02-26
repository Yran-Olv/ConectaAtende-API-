using System.Globalization;
using System.Text;
using ConectaAtende.Domain.Entities;
using ConectaAtende.Domain.Repositories;
using ConectaAtende.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ConectaAtende.Infrastructure.Repositories;

public class ContactRepository : IContactRepository
{
    private readonly ConectaAtendeDbContext _context;

    public ContactRepository(ConectaAtendeDbContext context)
    {
        _context = context;
    }

    public async Task<Contact?> GetByIdAsync(Guid id)
    {
        return await _context.Contacts.FindAsync(id);
    }

    public async Task<IEnumerable<Contact>> GetAllAsync(int page, int pageSize)
    {
        return await _context.Contacts
            .OrderBy(c => c.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<IEnumerable<Contact>> SearchByNameAsync(string name)
    {
        if (string.IsNullOrWhiteSpace(name) || name.Length < 3)
            return Enumerable.Empty<Contact>();

        var normalizedName = NormalizeString(name);
        var allContacts = await _context.Contacts.ToListAsync();
        
        var results = new HashSet<Contact>();
        foreach (var contact in allContacts)
        {
            var contactNormalizedName = NormalizeString(contact.Name);
            if (contactNormalizedName.Contains(normalizedName))
            {
                results.Add(contact);
            }
        }

        return results;
    }

    public async Task<IEnumerable<Contact>> SearchByPhoneAsync(string phone)
    {
        if (string.IsNullOrWhiteSpace(phone))
            return Enumerable.Empty<Contact>();

        var normalizedPhone = new string(phone.Where(char.IsDigit).ToArray());
        var allContacts = await _context.Contacts.ToListAsync();
        
        var results = new HashSet<Contact>();
        foreach (var contact in allContacts)
        {
            foreach (var contactPhone in contact.Phones)
            {
                var normalizedContactPhone = new string(contactPhone.Where(char.IsDigit).ToArray());
                if (normalizedContactPhone == normalizedPhone)
                {
                    results.Add(contact);
                    break;
                }
            }
        }

        return results;
    }

    public async Task<Contact> AddAsync(Contact contact)
    {
        _context.Contacts.Add(contact);
        await _context.SaveChangesAsync();
        return contact;
    }

    public async Task<Contact> UpdateAsync(Contact contact)
    {
        var existing = await _context.Contacts.FindAsync(contact.Id);
        if (existing == null)
            throw new KeyNotFoundException("Contato não encontrado");

        existing.Name = contact.Name;
        existing.Phones = contact.Phones;
        existing.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var contact = await _context.Contacts.FindAsync(id);
        if (contact == null)
            return false;

        _context.Contacts.Remove(contact);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.Contacts.AnyAsync(c => c.Id == id);
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
