using System.Globalization;

namespace Localizer.Abstractions;

public interface ITranslationTextProvider
{
    public bool UsesConsole();
    public Task<string> GetTranslationFor(string value, CultureInfo cultureInfo);
}