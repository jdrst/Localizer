using System.Globalization;
using Localizer.Core;
using Localizer.Core.Abstractions;
using Spectre.Console;

namespace Localizer.Infrastructure.Provider;

internal class PromptTranslationProvider(IAnsiConsole console) : ITranslationProvider
{
    public IReadOnlyList<Message> Messages => [];
    public bool UsesConsole => true;

    public Task<string[]> GetTranslationsAsync(string[] texts, CultureInfo cultureInfo, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(texts);
        
        var results = new string[texts.Length];
        foreach (var (idx, value) in texts.Index())
            results[idx] = console.Prompt(
                new TextPrompt<string>(
                    $"Please provide missing translation for '[bold]{value}[/]' in culture: '{(string.IsNullOrWhiteSpace(cultureInfo.Name) ? "invariant" : cultureInfo.Name)}'.")
            );

        return Task.FromResult(results);
    }
}