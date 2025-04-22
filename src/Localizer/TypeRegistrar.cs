using Microsoft.Extensions.DependencyInjection;
using Spectre.Console.Cli;

namespace Localizer;

public class TypeRegistrar(IServiceCollection services) : ITypeRegistrar
{
    public void Register(Type service, Type implementation) 
        => services.AddSingleton(service, implementation);

    public void RegisterInstance(Type service, object implementation) 
        => services.AddSingleton(service, implementation);

    public void RegisterLazy(Type service, Func<object> factory)
    {
        ArgumentNullException.ThrowIfNull(factory);
        services.AddSingleton(service, _ => factory());
    }

    public ITypeResolver Build() 
        => new TypeResolver(services.BuildServiceProvider());
}

public sealed class TypeResolver(IServiceProvider provider) : ITypeResolver, IDisposable
{
    private readonly IServiceProvider _provider = provider ?? throw new ArgumentNullException(nameof(provider));

    public object? Resolve(Type? type)
    {
        if (type is null)
            return null;

        return _provider.GetService(type);
    }

    public void Dispose()
    {
        if (_provider is IDisposable disposable)
            disposable.Dispose();
    }
}