using System.ComponentModel;
using System.Text.Json;
using System.Text.Json.Nodes;
using Localizer.Application.Abstractions;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Localizer.Application.Commands.Config;

internal class SetCommand(IAnsiConsole console, IConfigValueSetter configValueSetter) : AsyncCommand<SetCommand.Settings> 
{
    public const string Name = "set";
    public const string Description = "Sets the given config key to the given value."; 
    public const string Example = "config set DeepL:Context 'This is additional context'"; 

    internal class Settings : CommandSettings
    {
        [Description("Config key to be set. For nested values use ':' as separator.")] 
        [CommandArgument(0, "[config key]")] 
        public string Key { get; set; } = string.Empty;
        
        [Description("Config value the key should be set to. Leave empty to unset.")]
        [CommandArgument(1, "[config value]")] 
        public string? Value { get; set; } = string.Empty;
        
        [Description("Set global or local config")] 
        [CommandOption("-g|--global")]
        public bool Global { get; set; }
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        await configValueSetter.SetValueAsync(settings.Key, settings.Value, settings.Global);
        console.WriteLine($"Set {settings.Key} to '{settings.Value}'" );
        return 0;
    }
}