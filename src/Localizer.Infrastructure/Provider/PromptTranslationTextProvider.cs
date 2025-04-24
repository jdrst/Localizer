using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Localizer.Core.Abstractions;
using Spectre.Console;

namespace Localizer.Infrastructure.Provider;

public class PromptTranslationTextProvider(IAnsiConsole console) : ITranslationTextProvider
{
    [SuppressMessage("Performance", "CA1822:Mark members as static")]
    public bool UsesConsole() => true;

    public Task<string> GetTranslationFor(string value, CultureInfo cultureInfo, CancellationToken ct = default)
    {
        return Task.FromResult(console.Prompt(
            new TextPrompt<string>($"Please provide missing translation for '[bold]{value}[/]' in culture: '{(string.IsNullOrWhiteSpace(cultureInfo?.Name) ? "invariant" : cultureInfo.Name)}'.")
        ));
    }
}