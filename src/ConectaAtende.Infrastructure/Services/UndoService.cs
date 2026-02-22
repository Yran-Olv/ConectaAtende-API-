using ConectaAtende.Domain.Entities;
using ConectaAtende.Domain.Repositories;

namespace ConectaAtende.Infrastructure.Services;

public enum UndoOperationType
{
    Create,
    Update,
    Delete
}

public class UndoOperation
{
    public UndoOperationType Type { get; set; }
    public Contact? Contact { get; set; }
    public DateTime Timestamp { get; set; }
}

/// <summary>
/// Implementação do mecanismo de undo usando Stack (pilha LIFO).
/// Responsabilidade na camada Infrastructure, conforme Clean Architecture.
/// </summary>
public class UndoService
{
    private readonly IContactRepository _repository;
    private readonly Stack<UndoOperation> _undoStack = new();

    public UndoService(IContactRepository repository)
    {
        _repository = repository;
    }

    public void RecordCreate(Contact contact)
    {
        _undoStack.Push(new UndoOperation
        {
            Type = UndoOperationType.Create,
            Contact = CloneContact(contact),
            Timestamp = DateTime.UtcNow
        });
    }

    public void RecordUpdate(Contact contact)
    {
        _undoStack.Push(new UndoOperation
        {
            Type = UndoOperationType.Update,
            Contact = CloneContact(contact),
            Timestamp = DateTime.UtcNow
        });
    }

    public void RecordDelete(Contact contact)
    {
        _undoStack.Push(new UndoOperation
        {
            Type = UndoOperationType.Delete,
            Contact = CloneContact(contact),
            Timestamp = DateTime.UtcNow
        });
    }

    public bool HasPendingOperations => _undoStack.Count > 0;

    public async Task<bool> UndoAsync()
    {
        if (_undoStack.Count == 0)
            return false;

        var operation = _undoStack.Pop();

        switch (operation.Type)
        {
            case UndoOperationType.Create:
                if (operation.Contact != null)
                    await _repository.DeleteAsync(operation.Contact.Id);
                break;

            case UndoOperationType.Update:
                if (operation.Contact != null)
                {
                    var existing = await _repository.GetByIdAsync(operation.Contact.Id);
                    if (existing != null)
                    {
                        existing.Update(operation.Contact.Name, operation.Contact.Phones);
                        await _repository.UpdateAsync(existing);
                    }
                }
                break;

            case UndoOperationType.Delete:
                if (operation.Contact != null)
                    await _repository.AddAsync(operation.Contact);
                break;
        }

        return true;
    }

    private static Contact CloneContact(Contact contact)
    {
        return new Contact
        {
            Id = contact.Id,
            Name = contact.Name,
            Phones = new List<string>(contact.Phones),
            CreatedAt = contact.CreatedAt,
            UpdatedAt = contact.UpdatedAt
        };
    }
}
