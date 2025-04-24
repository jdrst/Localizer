using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using DeepL;
using Localizer.Application.Abstractions;
using Localizer.Core.Abstractions;
using Localizer.Infrastructure.Logging;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Localizer.Infrastructure.Provider.DeepL;

public sealed class DeepLTranslationTextProvider : ITranslationTextProvider, IDisposable
{
    private readonly ILogger<DeepLTranslationTextProvider> _logger;

    [SuppressMessage("Performance", "CA1822:Mark members as static")]
    public bool UsesConsole() => false;
    
    private int _charactersBilled;
    private readonly ITranslator _client;
    private readonly TextTranslateOptions _translateOptions;
    private readonly string? _sourceLanguage;


    public DeepLTranslationTextProvider(ILogger<DeepLTranslationTextProvider> logger, IOptions<DeepLOptions> options, ITranslator deepLClient, IAppInfo appInfo)
    {
        _logger = logger;
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(appInfo);

        _client = deepLClient;
        _sourceLanguage = options.Value?.SourceLanguage;
        _translateOptions = new TextTranslateOptions
        {
            Context = $"We are translating a c# string that might be in composite format. {options.Value?.Context}"
        };
    }

    public async Task<string> GetTranslationFor(string value, CultureInfo cultureInfo, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(cultureInfo);
        //TODO: errorhandling?
        var result = await _client.TranslateTextAsync(value, _sourceLanguage, cultureInfo.Name,
            _translateOptions, ct);
        _charactersBilled += result.BilledCharacters;
        _logger.DebugCharactersUsed(_charactersBilled);
        return result.Text;
    }

    public void Dispose()
    {
        _client.Dispose();
    }
}