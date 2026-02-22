using ConectaAtende.Domain.Entities;

namespace ConectaAtende.UnitTests.Domain;

public class TicketTests
{
    [Fact]
    public void Create_Ticket_Should_Set_Default_Values()
    {
        // Arrange
        var contactId = Guid.NewGuid();

        // Act
        var ticket = new Ticket(contactId, "Problema com login", TicketPriority.High);

        // Assert
        Assert.NotEqual(Guid.Empty, ticket.Id);
        Assert.Equal(contactId, ticket.ContactId);
        Assert.Equal("Problema com login", ticket.Description);
        Assert.Equal(TicketPriority.High, ticket.Priority);
        Assert.Equal(TicketStatus.Created, ticket.Status);
        Assert.True(ticket.CreatedAt <= DateTime.UtcNow);
        Assert.Null(ticket.QueuedAt);
        Assert.Null(ticket.DequeuedAt);
    }

    [Fact]
    public void Enqueue_Ticket_Should_Change_Status_To_Queued()
    {
        // Arrange
        var ticket = new Ticket(Guid.NewGuid(), "Teste", TicketPriority.Medium);

        // Act
        ticket.Enqueue();

        // Assert
        Assert.Equal(TicketStatus.Queued, ticket.Status);
        Assert.NotNull(ticket.QueuedAt);
        Assert.True(ticket.QueuedAt <= DateTime.UtcNow);
    }

    [Fact]
    public void Enqueue_Ticket_Twice_Should_Throw()
    {
        // Arrange
        var ticket = new Ticket(Guid.NewGuid(), "Teste", TicketPriority.Medium);
        ticket.Enqueue();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => ticket.Enqueue());
    }

    [Fact]
    public void Dequeue_Ticket_Should_Change_Status_To_InProgress()
    {
        // Arrange
        var ticket = new Ticket(Guid.NewGuid(), "Teste", TicketPriority.Medium);
        ticket.Enqueue();

        // Act
        ticket.Dequeue();

        // Assert
        Assert.Equal(TicketStatus.InProgress, ticket.Status);
        Assert.NotNull(ticket.DequeuedAt);
        Assert.True(ticket.DequeuedAt <= DateTime.UtcNow);
    }

    [Fact]
    public void Dequeue_Ticket_Without_Enqueue_Should_Throw()
    {
        // Arrange
        var ticket = new Ticket(Guid.NewGuid(), "Teste", TicketPriority.Medium);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => ticket.Dequeue());
    }
}
