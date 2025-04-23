using Spectre.Console;

namespace Localizer.Application.Commands.Config.Helpers;

internal static class ConfigTable
{
    public static Table Create()
    {
        var table = new Table();
        table.AddColumn(string.Empty);
        table.AddColumn("Global");
        table.AddColumn("Local");
        table.ShowRowSeparators();
        table.Border(TableBorder.Markdown);
        return table;
    }

    public static Table AddSettingRow(this Table table, string key, string? globalValue, string? localValue)
    {
        if (!string.IsNullOrWhiteSpace(localValue))
            localValue = $"[bold]{localValue}[/]";
        else 
            globalValue = $"[bold]{globalValue}[/]";
        table.AddRow(key, globalValue ?? string.Empty, localValue ?? string.Empty);
        
        return table;
    }
}