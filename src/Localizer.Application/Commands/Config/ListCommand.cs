using Localizer.Application.Abstractions;
using Localizer.Application.Commands.Config.Helpers;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Localizer.Application.Commands.Config;

internal class ListCommand(IAnsiConsole console, IConfigValueGetter configValueGetter) : AsyncCommand<ListCommand.Settings> 
{
    public const string Name = "list";
    public const string Description = "Lists all set config values.";
    public const string Example = "config list";

    internal class Settings : CommandSettings;

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        var (globalValues, localValues) = await configValueGetter.ListValuesAsync();
        var table = ConfigTable.Create();

        foreach (var (key, value) in globalValues)
        {
            localValues.TryGetValue(key, out var localValue);
            table.AddSettingRow(key, value, localValue);
        }
        
        console.Write(table);

        return 0;
    }
}