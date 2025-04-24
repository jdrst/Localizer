using System.Globalization;
using Localizer.Core;
using Localizer.Core.Abstractions;

namespace Localizer.Infrastructure.Provider;

internal class ReplaceMeTranslationTextProvider : ITranslationTextProvider
{
    public IReadOnlyList<Message> Messages => [];
    
    public  bool UsesConsole => false;
    
    public const string ReplaceText = "<<replaceme>>";

    public Task<string> GetTranslationFor(string value, CultureInfo cultureInfo, CancellationToken ct = default) => Task.FromResult($"<<replaceme>> {value}");
}