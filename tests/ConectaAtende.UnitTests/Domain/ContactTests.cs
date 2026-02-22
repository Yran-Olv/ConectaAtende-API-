using ConectaAtende.Domain.Entities;

namespace ConectaAtende.UnitTests.Domain;

public class ContactTests
{
    [Fact]
    public void Create_Contact_Should_Set_Default_Values()
    {
        // Arrange & Act
        var contact = new Contact("João Silva", new List<string> { "11987654321" });

        // Assert
        Assert.NotEqual(Guid.Empty, contact.Id);
        Assert.Equal("João Silva", contact.Name);
        Assert.Single(contact.Phones);
        Assert.Equal("11987654321", contact.Phones.First());
        Assert.True(contact.CreatedAt <= DateTime.UtcNow);
        Assert.True(contact.UpdatedAt <= DateTime.UtcNow);
    }

    [Fact]
    public void Create_Contact_With_Multiple_Phones_Should_Succeed()
    {
        // Arrange
        var phones = new List<string> { "11987654321", "1133334444" };

        // Act
        var contact = new Contact("Maria Santos", phones);

        // Assert
        Assert.Equal(2, contact.Phones.Count);
        Assert.Contains("11987654321", contact.Phones);
        Assert.Contains("1133334444", contact.Phones);
    }

    [Fact]
    public void Create_Contact_With_Empty_Name_Should_Allow()
    {
        // Arrange & Act
        var contact1 = new Contact("", new List<string> { "11987654321" });
        var contact2 = new Contact("   ", new List<string> { "11987654321" });

        // Assert
        // A entidade Contact permite nome vazio (validação é feita na camada de aplicação)
        Assert.NotNull(contact1);
        Assert.NotNull(contact2);
        Assert.Equal("", contact1.Name);
        Assert.Equal("   ", contact2.Name);
    }
}
