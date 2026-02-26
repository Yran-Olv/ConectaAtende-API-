using ConectaAtende.Application.DTOs;
using ConectaAtende.Application.Services;
using ConectaAtende.Domain.Entities;
using ConectaAtende.Domain.Repositories;
using ConectaAtende.Domain.Services;
using ConectaAtende.Infrastructure.Services;
using ConectaAtende.UnitTests.TestHelpers;

namespace ConectaAtende.UnitTests.Application;

public class TicketServiceTests
{
    private readonly IContactRepository _contactRepository;
    private readonly ITicketRepository _ticketRepository;
    private readonly ITriagePolicyService _triagePolicyService;
    private readonly TicketService _service;

    public TicketServiceTests()
    {
        _contactRepository = new TestContactRepository();
        _ticketRepository = new TestTicketRepository();
        _triagePolicyService = new TriagePolicyService();
        _service = new TicketService(_ticketRepository, _contactRepository, _triagePolicyService);
    }

    private async Task<Guid> CreateContactAsync(string name = "Contato Teste")
    {
        var contact = new Contact(name, new List<string> { "11999999999" });
        await _contactRepository.AddAsync(contact);
        return contact.Id;
    }

    [Fact]
    public async Task CreateAsync_Valid_Ticket_Should_Succeed()
    {
        // Arrange
        var contactId = await CreateContactAsync();
        var dto = new CreateTicketDto
        {
            ContactId = contactId,
            Description = "Problema com login",
            Priority = "High"
        };

        // Act
        var result = await _service.CreateAsync(dto);

        // Assert
        Assert.NotNull(result);
        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.Equal(contactId, result.ContactId);
        Assert.Equal("Problema com login", result.Description);
        Assert.Equal("High", result.Priority);
        Assert.Equal("Created", result.Status);
    }

    [Fact]
    public async Task CreateAsync_Invalid_Contact_Should_Throw()
    {
        // Arrange
        var dto = new CreateTicketDto
        {
            ContactId = Guid.NewGuid(), // contato inexistente
            Description = "Teste",
            Priority = "Medium"
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateAsync(dto));
    }

    [Fact]
    public async Task EnqueueAsync_Should_Change_Status_To_Queued()
    {
        // Arrange
        var contactId = await CreateContactAsync();
        var ticket = await _service.CreateAsync(new CreateTicketDto
        {
            ContactId = contactId,
            Description = "Teste",
            Priority = "Medium"
        });

        // Act
        var result = await _service.EnqueueAsync(ticket.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Queued", result.Status);
        Assert.NotNull(result.QueuedAt);
    }

    [Fact]
    public async Task GetNextAsync_Should_Return_Next_Ticket_By_Policy()
    {
        // Arrange
        var contactId = await CreateContactAsync();
        var ticket = await _service.CreateAsync(new CreateTicketDto
        {
            ContactId = contactId,
            Description = "Teste",
            Priority = "Medium"
        });
        await _service.EnqueueAsync(ticket.Id);

        // Act
        var result = await _service.GetNextAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(ticket.Id, result.Id);
    }

    [Fact]
    public async Task DequeueAsync_Should_Change_Status_To_InProgress()
    {
        // Arrange
        var contactId = await CreateContactAsync();
        var ticket = await _service.CreateAsync(new CreateTicketDto
        {
            ContactId = contactId,
            Description = "Teste",
            Priority = "Medium"
        });
        await _service.EnqueueAsync(ticket.Id);

        // Act
        var result = await _service.DequeueAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal("InProgress", result.Status);
        Assert.NotNull(result.DequeuedAt);
    }

    [Fact]
    public async Task GetNextAsync_EmptyQueue_Should_Return_Null()
    {
        // Act
        var result = await _service.GetNextAsync();

        // Assert
        Assert.Null(result);
    }
}
