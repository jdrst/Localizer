using System.ComponentModel;
using System.Text.Json;
using System.Text.Json.Nodes;
using Localizer.Infrastructure;
using Localizer.Infrastructure.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Localizer.Commands.Config;

internal class SetCommand(IAnsiConsole console, IConfigurationManager configurationManager) : AsyncCommand<SetCommand.Settings> 
{
    public const string Name = "set";
    public const string Description = "foo bar"; // TODO
    public const string Example = "set"; // TODO
    private readonly JsonSerializerOptions _jsonSerializerOptions = new(){ WriteIndented = true };

    internal class Settings : CommandSettings
    {
        [Description("")] // TODO
        [CommandArgument(0, "[config path]")] 
        public string Path { get; set; } = string.Empty;
        
        [Description("")] // TODO
        [CommandArgument(1, "[config value]")] 
        public string Value { get; set; } = string.Empty;
        
        [Description("")] // TODO
        [CommandOption("-g|--global")]
        public bool Global { get; set; }
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        var provider = (configurationManager as IConfigurationRoot)!.Providers.Single(p =>
            p is JsonConfigurationProvider provider && provider.Source.Path ==
            (settings.Global ? Paths.AppSettingsFile : Paths.LocalConfigFile)) as JsonConfigurationProvider;

        JsonNode node;
        await using (var reader = File.OpenRead(provider!.Source.Path!))
        {
            node = (await JsonNode.ParseAsync(reader))!;
        }

        var parts = settings.Path.Split(':');

        if (parts.Length > 1)
        {
            var parent = node;
            foreach (var part in parts[..^1])
            {
                parent![part] = new JsonObject();
                parent = parent[part];
            }
            parent![parts.Last()] = settings.Value;
        }
        else
            node[settings.Path] = settings.Value;

        
        var json = node.ToJsonString(_jsonSerializerOptions);
        await File.WriteAllTextAsync(provider.Source.Path!, json);
        console.WriteLine($"Set {settings.Path} to '{settings.Value}'" );

        return 1;
    }
}