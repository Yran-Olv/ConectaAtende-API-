using ConectaAtende.Application.DTOs;
using ConectaAtende.Application.Services;
using ConectaAtende.Domain.Repositories;
using ConectaAtende.UnitTests.TestHelpers;

namespace ConectaAtende.UnitTests.Application;

public class ContactServiceTests
{
    private readonly IContactRepository _repository;
    private readonly ContactService _service;

    public ContactServiceTests()
    {
        _repository = new TestContactRepository();
        _service = new ContactService(_repository);
    }

    [Fact]
    public async Task CreateAsync_Valid_Contact_Should_Succeed()
    {
        // Arrange
        var dto = new CreateContactDto
        {
            Name = "João Silva",
            Phones = new List<string> { "11987654321" }
        };

        // Act
        var result = await _service.CreateAsync(dto);

        // Assert
        Assert.NotNull(result);
        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.Equal("João Silva", result.Name);
        Assert.Single(result.Phones);
    }

    [Fact]
    public async Task CreateAsync_Empty_Name_Should_Throw()
    {
        // Arrange
        var dto = new CreateContactDto
        {
            Name = "",
            Phones = new List<string> { "11987654321" }
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateAsync(dto));
    }

    [Fact]
    public async Task GetByIdAsync_Existing_Contact_Should_Return_Contact()
    {
        // Arrange
        var dto = new CreateContactDto
        {
            Name = "Maria Santos",
            Phones = new List<string> { "11987654321" }
        };
        var created = await _service.CreateAsync(dto);

        // Act
        var result = await _service.GetByIdAsync(created.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(created.Id, result.Id);
        Assert.Equal("Maria Santos", result.Name);
    }

    [Fact]
    public async Task GetByIdAsync_NonExistent_Contact_Should_Return_Null()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var result = await _service.GetByIdAsync(nonExistentId);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task SearchByNameAsync_Should_Find_Contacts()
    {
        // Arrange
        await _service.CreateAsync(new CreateContactDto { Name = "João Silva", Phones = new List<string> { "11987654321" } });
        await _service.CreateAsync(new CreateContactDto { Name = "João Santos", Phones = new List<string> { "11987654322" } });
        await _service.CreateAsync(new CreateContactDto { Name = "Maria Silva", Phones = new List<string> { "11987654323" } });

        // Act
        var results = await _service.SearchByNameAsync("João");

        // Assert
        Assert.NotNull(results);
        Assert.Equal(2, results.Count());
    }

    [Fact]
    public async Task SearchByNameAsync_Less_Than_3_Characters_Should_Return_Empty()
    {
        // Arrange
        await _service.CreateAsync(new CreateContactDto { Name = "João Silva", Phones = new List<string> { "11987654321" } });

        // Act
        var results = await _service.SearchByNameAsync("Jo");

        // Assert
        Assert.Empty(results);
    }
}
