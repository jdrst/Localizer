using System.Globalization;

namespace Localizer.Core.Abstractions;

public interface ITranslationTextProvider
{
    public bool UsesConsole();
    public Task<string> GetTranslationFor(string value, CultureInfo cultureInfo);
}