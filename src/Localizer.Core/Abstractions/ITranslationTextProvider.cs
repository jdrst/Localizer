using System.Globalization;

namespace Localizer.Abstractions;

public interface ITranslationTextProvider
{
    public string GetTranslationFor(string value, CultureInfo cultureInfo);
}