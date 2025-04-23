using System.Diagnostics.CodeAnalysis;
using Localizer.Application.Commands;
using Localizer.Application.Commands.Config;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console.Cli;

namespace Localizer.Application;

[SuppressMessage("Globalization", "CA1308:Normalize strings to uppercase")]
public static class DependencyInjections
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        
        return services;
    }

    public static IConfigurator AddCommands(this IConfigurator cfg)
    {
        ArgumentNullException.ThrowIfNull(cfg);
        
        cfg.AddCommand<TranslateCommand>(TranslateCommand.Name)
            .WithDescription(TranslateCommand.Description)
            .WithExample(TranslateCommand.Example);
        cfg.AddBranch(nameof(Commands.Config).ToLowerInvariant(), branch =>
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
        return cfg;
    }
}