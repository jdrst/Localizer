using Localizer;
using Localizer.Application;
using Localizer.Application.Abstractions;
using Localizer.Core.Abstractions;
using Localizer.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console;
using Spectre.Console.Cli;

var services = new ServiceCollection();
services
    .AddApplication()
    .AddInfrastructure()
    .AddConfiguration()
    .AddSingleton<IAppInfo, AppInfo>();

var registrar = new TypeRegistrar(services);

var app = new CommandApp(registrar);
app.Configure(cfg =>
{
    cfg.AddCommands()
        .SetExceptionHandler((ex, resolver) =>
        {
            AnsiConsole.WriteException(ex, ExceptionFormats.ShortenPaths);
        });
});

await app.RunAsync(args);