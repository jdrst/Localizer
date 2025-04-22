using System.Globalization;
using Localizer.Abstractions;

namespace Localizer.Provider;

internal class ReplaceMeTranslationTextProvider : ITranslationTextProvider
{
    public bool UsesConsole() => false;
    // public Task<string> GetTranslationFor(string value, CultureInfo cultureInfo) => Task.FromResult($"<<replaceme>> {value}");
    public async Task<string> GetTranslationFor(string value, CultureInfo cultureInfo)
    {
        await Task.Delay(1000); //TODO 
        return $"<<replaceme>> {value}";
    }
}