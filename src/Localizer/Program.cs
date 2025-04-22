using Localizer;
using Localizer.Commands;
using Localizer.Commands.Config;
using Localizer.Core.Abstractions;
using Localizer.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console;
using Spectre.Console.Cli;

var services = new ServiceCollection();
services.AddSingleton<IAppInfo, AppInfo>();
services
    .AddInfrastructure()
    .AddConfiguration();

var registrar = new TypeRegistrar(services);

var app = new CommandApp(registrar);
app.Configure(cfg =>
{
    cfg.AddCommand<TranslateCommand>(TranslateCommand.Name)
        .WithDescription(TranslateCommand.Description)
        .WithExample(TranslateCommand.Example);
#pragma warning disable CA1308
    cfg.AddBranch(nameof(Localizer.Commands.Config).ToLowerInvariant(), branch =>
#pragma warning restore CA1308
    {
        branch.AddCommand<ListCommand>(ListCommand.Name)
            .WithDescription(ListCommand.Description)
            .WithExample(ListCommand.Example);
        branch.AddCommand<GetCommand>(GetCommand.Name)
            .WithDescription(GetCommand.Description)
            .WithExample(GetCommand.Example);
        branch.AddCommand<SetCommand>(SetCommand.Name)
            .WithDescription(SetCommand.Description)
            .WithExample(SetCommand.Example);
    });
    cfg.SetExceptionHandler((ex, resolver) =>
    {
        AnsiConsole.WriteException(ex, ExceptionFormats.ShortenPaths);
    });
});

await app.RunAsync(args);