using System.ComponentModel;
using Localizer.Infrastructure;
using Localizer.Infrastructure.Provider.DeepL;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Localizer.Commands.Config;

internal class GetCommand(IAnsiConsole console, IConfigurationManager configurationManager) : AsyncCommand<GetCommand.Settings> 
{
    public const string Name = "get";
    public const string Description = "Gets a configuration value"; // TODO
    public const string Example = $"config get {nameof(DeepL)}:{nameof(DeepLOptions.Context)}"; // TODO
    internal class Settings : CommandSettings
    {
        [Description("")] // TODO
        [CommandArgument(0, "[config path]")] 
        public string Path { get; set; } = string.Empty;
        
        [Description("")] // TODO
        [CommandOption("-g|--global")]
        public bool Global { get; set; }
    }

    public override Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        var root = configurationManager as IConfigurationRoot;
        var globalProvider = root!.Providers.Single(p => p is JsonConfigurationProvider { Source.Path: Paths.AppSettingsFile });
        if (settings.Global)
        {
            if (globalProvider.TryGet(settings.Path, out var value))
            {
                console.WriteLine($"{settings.Path} = {value}");
                return Task.FromResult(0);
            }
        }
        else
        {
            var localProvider = root.Providers.Single(p => p is JsonConfigurationProvider { Source.Path: Paths.LocalConfigFile });
            if (localProvider.TryGet(settings.Path, out var localValue))
            {
                console.WriteLine($"{settings.Path} = '{localValue}' (locally set)");
                return Task.FromResult(0);
            }
            if (globalProvider.TryGet(settings.Path, out var globalValue))
            {
                console.WriteLine($"{settings.Path} = '{globalValue}' (globally set)");
                return Task.FromResult(0);
            }
        }


        console.WriteLine("Config setting not found.");
        return Task.FromResult(-1);
    }
}