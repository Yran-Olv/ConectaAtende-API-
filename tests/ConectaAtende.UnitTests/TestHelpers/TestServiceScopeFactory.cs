using ConectaAtende.Domain.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace ConectaAtende.UnitTests.TestHelpers;

/// <summary>
/// Implementação simplificada de IServiceScopeFactory para testes.
/// </summary>
public class TestServiceScopeFactory : IServiceScopeFactory
{
    private readonly IContactRepository _contactRepository;

    public TestServiceScopeFactory(IContactRepository contactRepository)
    {
        _contactRepository = contactRepository;
    }

    public IServiceScope CreateScope()
    {
        var serviceProvider = new TestServiceProvider(_contactRepository);
        return new TestServiceScope(serviceProvider);
    }
}

public class TestServiceProvider : IServiceProvider
{
    private readonly IContactRepository _contactRepository;

    public TestServiceProvider(IContactRepository contactRepository)
    {
        _contactRepository = contactRepository;
    }

    public object? GetService(Type serviceType)
    {
        if (serviceType == typeof(IContactRepository))
            return _contactRepository;

        return null;
    }
}

public class TestServiceScope : IServiceScope
{
    public IServiceProvider ServiceProvider { get; }

    public TestServiceScope(IServiceProvider serviceProvider)
    {
        ServiceProvider = serviceProvider;
    }

    public void Dispose()
    {
        // Nada a fazer em testes
    }
}
