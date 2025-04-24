using Localizer;
using Localizer.Application;
using Localizer.Application.Abstractions;
using Localizer.Application.Commands.Config.Helpers;
using Localizer.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Spectre.Console;
using Spectre.Console.Cli;


var registrar = new TypeRegistrar(Services.Create());

var app = new CommandApp(registrar);
app.Configure(cfg =>
{
    cfg.AddCommands()
        .SetExceptionHandler((ex, resolver) =>
        {
            if (ex.InnerException is OptionsValidationException)
            {
                AnsiConsole.Write(new Markup($"{ConsoleExtensions.ErrorMarkup}{ex.InnerException.Message}"));
                return;
            }
            if (ex is CommandAppException)
            {
                AnsiConsole.Write(new Markup($"{ConsoleExtensions.ErrorMarkup}{ex.Message}"));
                return;
            }
            AnsiConsole.WriteException(ex, ExceptionFormats.ShortenPaths);
        });
});

await app.RunAsync(args);

internal static class Services
{
    public static IServiceCollection Create()
    {
        var services = new ServiceCollection();
        services
            .AddApplication()
            .AddInfrastructure()
            .AddSingleton<IAppInfo, AppInfo>();
        return services;
    }
}