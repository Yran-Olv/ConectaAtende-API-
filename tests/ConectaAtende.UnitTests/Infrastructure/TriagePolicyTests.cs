using ConectaAtende.Domain.Entities;
using ConectaAtende.Infrastructure.Services;
using ConectaAtende.Infrastructure.Services.TriagePolicies;

namespace ConectaAtende.UnitTests.Infrastructure;

public class TriagePolicyTests
{
    private static List<Ticket> CreateTickets()
    {
        var contactId = Guid.NewGuid();
        var tickets = new List<Ticket>
        {
            new Ticket(contactId, "Ticket Low",    TicketPriority.Low),
            new Ticket(contactId, "Ticket Medium", TicketPriority.Medium),
            new Ticket(contactId, "Ticket High",   TicketPriority.High),
        };
        foreach (var t in tickets) t.Enqueue();
        return tickets;
    }

    [Fact]
    public void FirstComeFirstServed_Should_Return_Oldest_Ticket()
    {
        // Arrange
        var policy = new FirstComeFirstServedPolicy();
        var tickets = CreateTickets();
        var oldest = tickets.First();

        // Act
        var result = policy.SelectNext(tickets);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(oldest.Id, result.Id);
    }

    [Fact]
    public void Priority_Policy_Should_Return_High_Priority_First()
    {
        // Arrange
        var policy = new PriorityPolicy();
        var tickets = CreateTickets();

        // Act
        var result = policy.SelectNext(tickets);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(TicketPriority.High, result.Priority);
    }

    [Fact]
    public void Mixed_Policy_Should_Return_High_Priority_First()
    {
        // Arrange
        var policy = new MixedPolicy();
        var tickets = CreateTickets();

        // Act
        var result = policy.SelectNext(tickets);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(TicketPriority.High, result.Priority);
    }

    [Fact]
    public void Mixed_Policy_Without_High_Should_Return_Medium()
    {
        // Arrange
        var policy = new MixedPolicy();
        var contactId = Guid.NewGuid();
        var tickets = new List<Ticket>
        {
            new Ticket(contactId, "Low 1",    TicketPriority.Low),
            new Ticket(contactId, "Medium 1", TicketPriority.Medium),
        };
        foreach (var t in tickets) t.Enqueue();

        // Act
        var result = policy.SelectNext(tickets);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(TicketPriority.Medium, result.Priority);
    }

    [Fact]
    public void TriagePolicyService_Should_Change_Policy_Dynamically()
    {
        // Arrange
        var service = new TriagePolicyService();

        // Act & Assert - padrão é FirstComeFirstServed
        Assert.Equal("FirstComeFirstServed", service.GetCurrentPolicyName());

        service.SetPolicy("Priority");
        Assert.Equal("Priority", service.GetCurrentPolicyName());

        service.SetPolicy("Mixed");
        Assert.Equal("Mixed", service.GetCurrentPolicyName());

        service.SetPolicy("FirstComeFirstServed");
        Assert.Equal("FirstComeFirstServed", service.GetCurrentPolicyName());
    }

    [Fact]
    public void TriagePolicyService_Invalid_Policy_Should_Throw()
    {
        // Arrange
        var service = new TriagePolicyService();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => service.SetPolicy("PolicyInexistente"));
    }
}
