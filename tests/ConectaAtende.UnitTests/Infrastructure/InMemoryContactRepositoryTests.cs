using ConectaAtende.Domain.Entities;
using ConectaAtende.Domain.Repositories;
using ConectaAtende.UnitTests.TestHelpers;

namespace ConectaAtende.UnitTests.Infrastructure;

public class InMemoryContactRepositoryTests
{
    private readonly IContactRepository _repository;

    public InMemoryContactRepositoryTests()
    {
        _repository = new TestContactRepository();
    }

    [Fact]
    public async Task AddAsync_Should_Store_Contact()
    {
        // Arrange
        var contact = new Contact("João Silva", new List<string> { "11987654321" });

        // Act
        var result = await _repository.AddAsync(contact);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(contact.Id, result.Id);
    }

    [Fact]
    public async Task GetByIdAsync_Existing_Contact_Should_Return_Contact()
    {
        // Arrange
        var contact = new Contact("Maria Santos", new List<string> { "11987654321" });
        await _repository.AddAsync(contact);

        // Act
        var result = await _repository.GetByIdAsync(contact.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(contact.Id, result.Id);
        Assert.Equal("Maria Santos", result.Name);
    }

    [Fact]
    public async Task UpdateAsync_Should_Update_Contact()
    {
        // Arrange
        var contact = new Contact("João Silva", new List<string> { "11987654321" });
        await _repository.AddAsync(contact);
        
        // Get the contact and update it
        var existingContact = await _repository.GetByIdAsync(contact.Id);
        Assert.NotNull(existingContact);
        existingContact.Name = "João Silva Santos";
        existingContact.Phones = new List<string> { "11987654321", "1133334444" };

        // Act
        var result = await _repository.UpdateAsync(existingContact);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("João Silva Santos", result.Name);
        Assert.Equal(2, result.Phones.Count);
    }

    [Fact]
    public async Task DeleteAsync_Should_Remove_Contact()
    {
        // Arrange
        var contact = new Contact("João Silva", new List<string> { "11987654321" });
        await _repository.AddAsync(contact);

        // Act
        var deleted = await _repository.DeleteAsync(contact.Id);
        var result = await _repository.GetByIdAsync(contact.Id);

        // Assert
        Assert.True(deleted);
        Assert.Null(result);
    }

    [Fact]
    public async Task SearchByPhoneAsync_Should_Find_Contact()
    {
        // Arrange
        var contact = new Contact("João Silva", new List<string> { "11987654321" });
        await _repository.AddAsync(contact);

        // Act
        var results = await _repository.SearchByPhoneAsync("11987654321");

        // Assert
        Assert.NotNull(results);
        Assert.Single(results);
        Assert.Equal(contact.Id, results.First().Id);
    }
}
