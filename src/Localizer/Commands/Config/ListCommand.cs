using Localizer.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Localizer.Commands.Config;

internal class ListCommand(IAnsiConsole console, IConfigurationManager configurationManager) : AsyncCommand<ListCommand.Settings> 
{
    public const string Name = "list";
    public const string Description = "foo bar"; //TODO
    public const string Example = "list";

    internal class Settings : CommandSettings;

    public override Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        var providers = (configurationManager as IConfigurationRoot)!.Providers.AsEnumerable();
        foreach (var provider in providers)
        {
            if (provider is not JsonConfigurationProvider jsonConfigurationProvider) continue;
            
            var isGlobal = jsonConfigurationProvider.Source.Path == Paths.AppSettingsFile;
                
            console.WriteLine(isGlobal ? "Global:" : "Local:");
                
            foreach (var (key, _) in configurationManager.AsEnumerable())
            {
                if (provider.TryGet(key, out var value))
                    if (isGlobal || !string.IsNullOrWhiteSpace(value))
                        console.WriteLine($"{key} = {value}");
            }
        }

        return Task.FromResult(0);
    }
}