using System.ComponentModel;
using Localizer.Application.Abstractions;
using Localizer.Application.Commands.Config.Helpers;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Localizer.Application.Commands.Config;

internal class GetCommand(IAnsiConsole console, IConfigValueGetter configValueGetter) : AsyncCommand<GetCommand.Settings> 
{
    public const string Name = "get";
    public const string Description = "Gets the configuration value for the given key."; 
    public const string Example = "config get DeepL:Context"; 
    internal class Settings : CommandSettings
    {
        [Description("Config key to get the value for. For nested values use ':' as separator.")] 
        [CommandArgument(0, "[config key]")] 
        public string Key { get; set; } = string.Empty;
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        var (globalValue, localValue) = await configValueGetter.GetValueAsync(settings.Key);

        if (localValue is null && globalValue is null)
        {
            console.WriteLine("Config setting not set or found.");
            return -1;
        } 
        
        var table = ConfigTable.Create();
        table.AddSettingRow(settings.Key, globalValue, localValue);
        console.Write(table);

        return 0;
    }
}