using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Localizer.Core.Abstractions;

namespace Localizer.Infrastructure.Provider;

internal class ReplaceMeTranslationTextProvider : ITranslationTextProvider
{
    // public Task<string> GetTranslationFor(string value, CultureInfo cultureInfo) => Task.FromResult($"<<replaceme>> {value}");
    [SuppressMessage("Performance", "CA1822:Mark members as static")]
    public  bool UsesConsole() => false;

    public async Task<string> GetTranslationFor(string value, CultureInfo cultureInfo)
    {
        await Task.Delay(1000); //TODO 
        return $"<<replaceme>> {value}";
    }
}