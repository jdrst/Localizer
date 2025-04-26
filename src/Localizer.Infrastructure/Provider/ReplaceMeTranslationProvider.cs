using System.Globalization;
using Localizer.Core;
using Localizer.Core.Abstractions;

namespace Localizer.Infrastructure.Provider;

internal class ReplaceMeTranslationProvider : ITranslationProvider
{
    public const string ReplaceText = "<<replaceme>>";
    public IReadOnlyList<Message> Messages => [];
    public bool UsesConsole => false;
    public Task<string[]> GetTranslationsAsync(string[] texts, CultureInfo cultureInfo, CancellationToken ct = default)
    {
        var results = new string[texts.Length];
        foreach (var (idx, value) in texts.Index())
            results[idx] = $"<<replaceme>> {value}";

        return Task.FromResult(results);
    }
}