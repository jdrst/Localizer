using Localizer;
using Localizer.Commands;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console.Cli;

var services = new ServiceCollection();
services.AddInfrastructure();

var registrar = new TypeRegistrar(services);

var app = new CommandApp(registrar);
app.Configure(cfg =>
    cfg.AddCommand<TranslateCommand>(TranslateCommand.Name)
            .WithDescription(TranslateCommand.Description)
            .WithExample(TranslateCommand.Example));

await app.RunAsync(args);