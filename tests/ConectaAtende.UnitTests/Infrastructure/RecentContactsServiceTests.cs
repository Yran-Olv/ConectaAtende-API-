using ConectaAtende.Domain.Entities;
using ConectaAtende.Domain.Repositories;
using ConectaAtende.Infrastructure.Services;
using ConectaAtende.UnitTests.TestHelpers;

namespace ConectaAtende.UnitTests.Infrastructure;

public class RecentContactsServiceTests
{
    private readonly IContactRepository _repository;
    private readonly RecentContactsService _service;

    public RecentContactsServiceTests()
    {
        _repository = new TestContactRepository();
        var scopeFactory = new TestServiceScopeFactory(_repository);
        _service = new RecentContactsService(scopeFactory, maxCapacity: 3);
    }

    private async Task<Guid> AddContactAsync(string name)
    {
        var contact = new Contact(name, new List<string> { "11999999999" });
        await _repository.AddAsync(contact);
        return contact.Id;
    }

    [Fact]
    public async Task AddRecent_Should_Appear_In_List()
    {
        // Arrange
        var id = await AddContactAsync("João Silva");
        _service.AddRecent(id);

        // Act
        var recent = await _service.GetRecentAsync(10);

        // Assert
        Assert.Single(recent);
        Assert.Equal(id, recent.First().Id);
    }

    [Fact]
    public async Task AddRecent_Same_Contact_Should_Move_To_Top()
    {
        // Arrange
        var id1 = await AddContactAsync("João");
        var id2 = await AddContactAsync("Maria");
        _service.AddRecent(id1);
        _service.AddRecent(id2);
        _service.AddRecent(id1); // acessa id1 novamente

        // Act
        var recent = (await _service.GetRecentAsync(10)).ToList();

        // Assert
        Assert.Equal(2, recent.Count);
        Assert.Equal(id1, recent[0].Id); // id1 deve estar no topo
    }

    [Fact]
    public async Task AddRecent_Exceeding_Capacity_Should_Remove_Oldest()
    {
        // Arrange (capacidade = 3)
        var id1 = await AddContactAsync("Contato 1");
        var id2 = await AddContactAsync("Contato 2");
        var id3 = await AddContactAsync("Contato 3");
        var id4 = await AddContactAsync("Contato 4");

        _service.AddRecent(id1);
        _service.AddRecent(id2);
        _service.AddRecent(id3);
        _service.AddRecent(id4); // deve expulsar id1

        // Act
        var recent = (await _service.GetRecentAsync(10)).ToList();

        // Assert
        Assert.Equal(3, recent.Count);
        Assert.DoesNotContain(recent, r => r.Id == id1); // id1 deve ter saído
        Assert.Contains(recent, r => r.Id == id4);
    }

    [Fact]
    public async Task RemoveRecent_Should_Remove_From_List()
    {
        // Arrange
        var id = await AddContactAsync("João Silva");
        _service.AddRecent(id);
        _service.RemoveRecent(id);

        // Act
        var recent = await _service.GetRecentAsync(10);

        // Assert
        Assert.Empty(recent);
    }

    [Fact]
    public async Task GetRecentAsync_With_Limit_Should_Respect_Limit()
    {
        // Arrange
        var id1 = await AddContactAsync("Contato 1");
        var id2 = await AddContactAsync("Contato 2");
        var id3 = await AddContactAsync("Contato 3");

        _service.AddRecent(id1);
        _service.AddRecent(id2);
        _service.AddRecent(id3);

        // Act
        var recent = (await _service.GetRecentAsync(2)).ToList();

        // Assert
        Assert.Equal(2, recent.Count);
    }
}
