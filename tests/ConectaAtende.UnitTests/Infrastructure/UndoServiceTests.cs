using ConectaAtende.Domain.Entities;
using ConectaAtende.Domain.Repositories;
using ConectaAtende.Infrastructure.Repositories;
using ConectaAtende.Infrastructure.Services;

namespace ConectaAtende.UnitTests.Infrastructure;

public class UndoServiceTests
{
    private readonly IContactRepository _repository;
    private readonly UndoService _undoService;

    public UndoServiceTests()
    {
        _repository = new InMemoryContactRepository();
        _undoService = new UndoService(_repository);
    }

    [Fact]
    public async Task Undo_Without_Operations_Should_Return_False()
    {
        // Act
        var result = await _undoService.UndoAsync();

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task Undo_Create_Should_Delete_Contact()
    {
        // Arrange
        var contact = new Contact("João Silva", new List<string> { "11987654321" });
        await _repository.AddAsync(contact);
        _undoService.RecordCreate(contact);

        // Act
        var undone = await _undoService.UndoAsync();
        var result = await _repository.GetByIdAsync(contact.Id);

        // Assert
        Assert.True(undone);
        Assert.Null(result); // deve ter sido removido
    }

    [Fact]
    public async Task Undo_Delete_Should_Restore_Contact()
    {
        // Arrange
        var contact = new Contact("Maria Santos", new List<string> { "11987654321" });
        await _repository.AddAsync(contact);
        _undoService.RecordDelete(contact);
        await _repository.DeleteAsync(contact.Id);

        // Act
        var undone = await _undoService.UndoAsync();
        var result = await _repository.GetByIdAsync(contact.Id);

        // Assert
        Assert.True(undone);
        Assert.NotNull(result); // deve ter sido restaurado
        Assert.Equal("Maria Santos", result.Name);
    }

    [Fact]
    public async Task Undo_Update_Should_Restore_Previous_State()
    {
        // Arrange
        var contact = new Contact("Pedro Lima", new List<string> { "11987654321" });
        await _repository.AddAsync(contact);
        _undoService.RecordUpdate(contact); // grava estado ANTES do update

        contact.Update("Pedro Lima Alterado", new List<string> { "11999999999" });
        await _repository.UpdateAsync(contact);

        // Act
        var undone = await _undoService.UndoAsync();
        var result = await _repository.GetByIdAsync(contact.Id);

        // Assert
        Assert.True(undone);
        Assert.NotNull(result);
        Assert.Equal("Pedro Lima", result.Name); // deve ter voltado ao nome original
    }

    [Fact]
    public void HasPendingOperations_After_Record_Should_Be_True()
    {
        // Arrange
        var contact = new Contact("Teste", new List<string> { "11987654321" });
        _undoService.RecordCreate(contact);

        // Assert
        Assert.True(_undoService.HasPendingOperations);
    }
}
