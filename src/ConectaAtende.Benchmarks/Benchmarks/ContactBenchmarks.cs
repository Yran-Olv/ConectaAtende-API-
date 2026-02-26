using BenchmarkDotNet.Attributes;
using ConectaAtende.Application.DTOs;
using ConectaAtende.Application.Services;
using ConectaAtende.Benchmarks.TestHelpers;
using ConectaAtende.Domain.Repositories;

namespace ConectaAtende.Benchmarks.Benchmarks;

[MemoryDiagnoser]
[SimpleJob]
public class ContactBenchmarks
{
    private ContactService _contactService = null!;
    private IContactRepository _repository = null!;
    private List<Guid> _contactIds = new();

    [GlobalSetup]
    public void Setup()
    {
        // Para benchmarks, usar repositório simplificado (em memória para performance)
        _repository = new BenchmarkContactRepository();
        _contactService = new ContactService(_repository);

        // Pré-popular com dados para os benchmarks
        var random = new Random(42);
        var firstNames = new[] { "João", "Maria", "Pedro", "Ana", "Carlos", "Julia", "Lucas", "Fernanda", "Rafael", "Mariana" };
        var lastNames = new[] { "Silva", "Santos", "Oliveira", "Souza", "Costa", "Pereira", "Rodrigues", "Almeida", "Nascimento", "Lima" };

        for (int i = 0; i < 10000; i++)
        {
            var firstName = firstNames[random.Next(firstNames.Length)];
            var lastName = lastNames[random.Next(lastNames.Length)];
            var name = $"{firstName} {lastName}";
            
            var phoneCount = random.Next(1, 3);
            var phones = new List<string>();
            for (int j = 0; j < phoneCount; j++)
            {
                var phone = $"{random.Next(10, 99)}{random.Next(100000000, 999999999)}";
                phones.Add(phone);
            }

            var dto = new CreateContactDto
            {
                Name = name,
                Phones = phones
            };

            var contact = _contactService.CreateAsync(dto).Result;
            _contactIds.Add(contact.Id);
        }
    }

    [Benchmark]
    public void InsertContacts()
    {
        var random = new Random();
        var firstNames = new[] { "João", "Maria", "Pedro", "Ana", "Carlos" };
        var lastNames = new[] { "Silva", "Santos", "Oliveira", "Souza", "Costa" };

        for (int i = 0; i < 1000; i++)
        {
            var firstName = firstNames[random.Next(firstNames.Length)];
            var lastName = lastNames[random.Next(lastNames.Length)];
            var name = $"{firstName} {lastName}";
            
            var phone = $"{random.Next(10, 99)}{random.Next(100000000, 999999999)}";
            var dto = new CreateContactDto
            {
                Name = name,
                Phones = new List<string> { phone }
            };

            _contactService.CreateAsync(dto).Wait();
        }
    }

    [Benchmark]
    public void SearchByName()
    {
        var searchTerms = new[] { "João", "Maria", "Pedro", "Ana", "Carlos" };
        var random = new Random();

        for (int i = 0; i < 100; i++)
        {
            var term = searchTerms[random.Next(searchTerms.Length)];
            _contactService.SearchByNameAsync(term).Wait();
        }
    }

    [Benchmark]
    public void SearchByPhone()
    {
        var random = new Random();
        var phonePrefixes = new[] { "11", "21", "31", "41", "51" };

        for (int i = 0; i < 100; i++)
        {
            var prefix = phonePrefixes[random.Next(phonePrefixes.Length)];
            var phone = $"{prefix}{random.Next(10000000, 99999999)}";
            _contactService.SearchByPhoneAsync(phone).Wait();
        }
    }

    [Benchmark]
    public void UpdateContacts()
    {
        var random = new Random();
        var newNames = new[] { "Novo Nome 1", "Novo Nome 2", "Novo Nome 3", "Novo Nome 4", "Novo Nome 5" };

        for (int i = 0; i < 100; i++)
        {
            if (_contactIds.Count == 0) break;

            var contactId = _contactIds[random.Next(_contactIds.Count)];
            var newName = newNames[random.Next(newNames.Length)];
            var updateDto = new UpdateContactDto
            {
                Name = newName,
                Phones = new List<string> { $"{random.Next(10, 99)}{random.Next(100000000, 999999999)}" }
            };

            _contactService.UpdateAsync(contactId, updateDto).Wait();
        }
    }

    [Benchmark]
    public void GetAllPaginated()
    {
        for (int page = 1; page <= 10; page++)
        {
            _contactService.GetAllAsync(page, 10).Wait();
        }
    }
}
