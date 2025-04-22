using System.Globalization;
using Localizer.Abstractions;

namespace Localizer.Provider;

public class ReplaceMeTranslationTextProvider : ITranslationTextProvider
{
    public string GetTranslationFor(string value, CultureInfo cultureInfo) => $"<<replaceme>> {value}";
}