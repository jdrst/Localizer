using System.Globalization;
using Localizer.Abstractions;

namespace Localizer;

internal class ReplaceMeTranslationTextProvider : ITranslationTextProvider
{
    public string GetTranslationFor(string value, CultureInfo cultureInfo) => $"<<replaceme>> {value}";
}