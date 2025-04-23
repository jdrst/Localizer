using System.ComponentModel;
using Localizer.Application.Abstractions;
using Localizer.Core;
using Localizer.Core.Abstractions;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Localizer.Application.Commands;

internal class TranslateCommand(IAnsiConsole console, IFileHandler fileHandler, ITranslationTextProvider translationTextProvider) : AsyncCommand<TranslateCommand.Settings>
{
    public const string Name = "translate";
    public const string Description = "foo bar"; //TODO
    public const string Example = "translate locale.json";
    internal class Settings : CommandSettings
    {
        [Description("Base file that is used by 'kli.Localize'")]
        [CommandArgument(0, "[base file]")]
        public string BaseFilePath { get; set; } = string.Empty;
        
        // [Description("Prompt for translations")]
        // [CommandOption("-i|--interactive")]
        // public bool Prompt { get; set; }
        
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
        
        if (!translationTextProvider.UsesConsole())
        {
            await console.Status().StartAsync("Translating...",async ctx =>
            {
                ctx.Spinner(Spinner.Known.Dots);
                ctx.SpinnerStyle(Style.Parse("yellow"));
                foreach (var to in culturedJsons)
                {
                    ctx.Status($"Translating to {to.CultureInfo.DisplayName}.");
                    await NodeInserter.InsertMissingNodes(baseObject, to.Json, translationTextProvider,
                        to.CultureInfo);
                }
            });
        }
        else
            foreach (var to in culturedJsons)
                await NodeInserter.InsertMissingNodes(baseObject, to.Json, translationTextProvider, to.CultureInfo);

        await fileHandler.WriteFilesAsync(settings.Prefix);
        
        return 0;
    }
}