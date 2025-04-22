using System.Globalization;
using Localizer.Abstractions;
using Spectre.Console;

namespace Localizer.Provider;

public class PromptTranslationTextProvider(IAnsiConsole console) : ITranslationTextProvider
{
    public bool UsesConsole() => true;

    public Task<string> GetTranslationFor(string value, CultureInfo cultureInfo)
    {
        return Task.FromResult(console.Prompt(
            new TextPrompt<string>($"Please provide missing translation for '{value}' in culture: '{(string.IsNullOrWhiteSpace(cultureInfo.Name) ? "invariant" : cultureInfo.Name)}'.")
        ));
    }
}