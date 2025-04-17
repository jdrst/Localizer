using System.Globalization;

namespace Localizer.Abstractions;

internal interface ITranslationTextProvider
{
    public string GetTranslationFor(string value, CultureInfo cultureInfo);
}