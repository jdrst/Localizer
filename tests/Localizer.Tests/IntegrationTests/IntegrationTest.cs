using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Localizer.Application;
using Localizer.Application.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Spectre.Console.Cli;
using Spectre.Console.Testing;

namespace Localizer.Tests.IntegrationTests;

[SuppressMessage("Maintainability", "CA1515:Consider making public types internal")]
public class IntegrationTest : IDisposable
{
    protected Mocks.TestPathProvider? TestPathProvider { get; private set; }
    protected LocaleFileProvider LocaleFileProvider { get; } = new();
    protected TestConsole TestConsole { get; } = new();
    protected CommandAppTester DefaultCommandAppTester([CallerMemberName] string methodName = "test")
    {
        var app = DefaultCommandAppBuilder(methodName).Build(TestConsole);
        TestPathProvider = TestPathProvider!.WithDefaultConfig();
        return app;
    }

    protected CommandAppTesterBuilder DefaultCommandAppBuilder([CallerMemberName] string methodName = "test")
    {
        var builder = new CommandAppTesterBuilder();
        TestPathProvider = new Mocks.TestPathProvider(methodName);
        builder.ReplaceService<IPathProvider>(TestPathProvider);
        return builder;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
    protected virtual void Dispose(bool disposing)
    {
        LocaleFileProvider.Dispose();
        TestConsole.Dispose();
        TestPathProvider?.Dispose();
    }
}

[SuppressMessage("Maintainability", "CA1515:Consider making public types internal")]
public class CommandAppTesterBuilder
{
    private readonly IServiceCollection _services = Services.Create();

    internal CommandAppTesterBuilder ReplaceService<T>(object instance)
    {
        _services.Replace(new ServiceDescriptor(typeof(T), instance));
        return this; 
    }
    
    internal CommandAppTester Build(TestConsole? console = null)
    {
        var typeRegistrar = new TypeRegistrar(_services);
        var commandAppTester = new CommandAppTester(typeRegistrar, null, console);
        commandAppTester.Configure(cfg => cfg.AddCommands().PropagateExceptions());
        return commandAppTester; 
    }
} 