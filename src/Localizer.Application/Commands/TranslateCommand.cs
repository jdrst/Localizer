using System.ComponentModel;
using System.Text.Json.Nodes;
using Localizer.Application.Abstractions;
using Localizer.Application.Commands.Config.Helpers;
using Localizer.Core;
using Localizer.Core.Abstractions;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Localizer.Application.Commands;

internal class TranslateCommand(IAnsiConsole console, IFileHandler fileHandler, ITranslationProvider translationProvider) : AsyncCommand<TranslateCommand.Settings>
{
    public const string Name = "translate";
    public const string Description = "Update and translate locale files according to the base file and their respective culture postfix.";
    public const string Example = "translate locale.json";
    internal class Settings : CommandSettings
    {
        [Description("Base file that is used by 'kli.Localize'. Relative to the current directory.")]
        [CommandArgument(0, "[base file]")]
        public string BaseFilePath { get; set; } = string.Empty;
        
        [Description("Prefix for the resulting files. If not specified files will be changed in place.")]
        [CommandOption("-p|--prefix")]
        public string? Prefix { get; set; }
    }

    public override async Task<int> ExecuteAsync(CommandContext ctx, Settings settings)
    {
        var success = await fileHandler.InitializeAsync(settings.BaseFilePath);

        if (!success)
        {
            foreach (var e in fileHandler.Messages())
                console.WriteLine(e.Text);
            return -1;
        }
        
        var baseObject = fileHandler.Base();

        var culturedJsons = fileHandler.CulturedJsons();
        
        if (!translationProvider.UsesConsole)
        {
            await console.Status().StartAsync("Translating...",async ctx =>
            {
                ctx.Spinner(Spinner.Known.Dots);
                ctx.SpinnerStyle(Style.Parse("yellow"));
                foreach (var to in culturedJsons)
                {
                    ctx.Status($"Translating to {to.CultureInfo.DisplayName}.");
                    await InsertAndTranslateAsync(baseObject, to);
                }
            });
        }
        else
            foreach (var to in culturedJsons)
                await InsertAndTranslateAsync(baseObject, to);
        
        await fileHandler.WriteFilesAsync(settings.Prefix);

        foreach (var message in translationProvider.Messages)
            console.WriteMessage(message);
        
        return 0;
    }

    private async Task InsertAndTranslateAsync(JsonObject from, CulturedJson to)
    {
        var (nodes, messages) = NodeInserter.InsertMissingNodes(from, to.Json);
        foreach (var message in messages)
            console.WriteMessage(message);
        await NodeTranslator.TranslateNodesAsync(nodes, translationProvider, to.CultureInfo);
    }
}