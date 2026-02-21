namespace ConectaAtende.Application.DTOs;

public class ContactDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<string> Phones { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class CreateContactDto
{
    public string Name { get; set; } = string.Empty;
    public List<string> Phones { get; set; } = new();
}

public class UpdateContactDto
{
    public string Name { get; set; } = string.Empty;
    public List<string> Phones { get; set; } = new();
}

public class PaginatedResult<T>
{
    public List<T> Items { get; set; } = new();
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
}
