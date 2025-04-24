using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Localizer.Core.Abstractions;

namespace Localizer.Infrastructure.Provider;

internal class ReplaceMeTranslationTextProvider : ITranslationTextProvider
{
    
    [SuppressMessage("Performance", "CA1822:Mark members as static")]
    public  bool UsesConsole() => false;
    
    public const string ReplaceText = "<<replaceme>>";

    public Task<string> GetTranslationFor(string value, CultureInfo cultureInfo, CancellationToken ct = default) => Task.FromResult($"<<replaceme>> {value}");
}