namespace ConectaAtende.Domain.Entities;

public class Contact
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<string> Phones { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public Contact()
    {
        Id = Guid.NewGuid();
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public Contact(string name, List<string> phones) : this()
    {
        Name = name;
        Phones = NormalizePhones(phones);
    }

    public void Update(string name, List<string> phones)
    {
        Name = name;
        Phones = NormalizePhones(phones);
        UpdatedAt = DateTime.UtcNow;
    }

    private static List<string> NormalizePhones(List<string> phones)
    {
        return phones.Select(phone => new string(phone.Where(char.IsDigit).ToArray())).ToList();
    }
}
