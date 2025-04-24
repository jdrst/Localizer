using System.Globalization;

namespace Localizer.Core.Abstractions;

public interface ITranslationTextProvider
{
    public IReadOnlyList<Message> Messages { get; }
    public bool UsesConsole { get; }
    public Task<string> GetTranslationFor(string value, CultureInfo cultureInfo, CancellationToken ct = default);
}