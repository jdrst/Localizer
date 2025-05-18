using System.Globalization;
using DeepL;
using Localizer.Application.Abstractions;
using Localizer.Core;
using Localizer.Core.Abstractions;
using Microsoft.Extensions.Options;

namespace Localizer.Infrastructure.Provider.DeepL;

internal class DeepLTranslationProvider : ITranslationProvider
{
    private const string CharactersBilled = "Characters billed this session: ";
    public IReadOnlyList<Message> Messages
    {
        get
        {
            //TODO: this is bad.
            if (!_messages.Any(msg => msg.Text.StartsWith(CharactersBilled, StringComparison.InvariantCulture)))
                    _messages.Add(Message.Info($"{CharactersBilled}{_charactersBilled}"));
            return _messages;
        }
    }

    public bool UsesConsole => false;

    private int _charactersBilled;
    private readonly ITranslator _client;
    private readonly TextTranslateOptions _translateOptions;
    private readonly string? _sourceLanguage;
    private readonly List<Message> _messages = [];

    public DeepLTranslationProvider(IOptions<DeepLOptions> options, ITranslator deepLClient, IAppInfo appInfo)
    {
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(appInfo);

        _client = deepLClient;
        _sourceLanguage = string.IsNullOrWhiteSpace(options.Value?.SourceLanguage) ? null : options.Value.SourceLanguage;
        _translateOptions = new TextTranslateOptions
        {
            Context = $"We are translating a c# string that might be in composite format. {options.Value?.Context}"
        };
    }
    
    public async Task<string[]> GetTranslationsAsync(string[] texts, CultureInfo cultureInfo, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(cultureInfo);
        ArgumentNullException.ThrowIfNull(texts);

        var targetLanguage = Adjust(cultureInfo.Name);

        try
        {
            var results = await _client.TranslateTextAsync(texts, _sourceLanguage, targetLanguage,
                _translateOptions, ct);
            var translations = new string[results.Length];
            foreach (var (idx, result) in results.Index())
            {
                _charactersBilled += result.BilledCharacters;
                translations[idx] = result.Text;
            }
            return translations;
        }
        catch (DeepLException ex)
        {
            return await FallbackAsync($"DeepL API Exception: {ex.Message}{Environment.NewLine}", texts, cultureInfo, ct);
        }
    }

    private async Task<string[]> FallbackAsync(string message, string[] texts, CultureInfo cultureInfo, CancellationToken ct = default)
    {
        var replaceMeProvider = new ReplaceMeTranslationProvider();
        _messages.Add(Message.Error($"{message} Inserting '{ReplaceMeTranslationProvider.ReplaceText}' instead of a translation."));
        return await replaceMeProvider.GetTranslationsAsync(texts, cultureInfo, ct);
    }

    //PT and EN are not supported, EN_(US/UK) PT_(BR/PT) are though 
    //see https://github.com/DeepLcom/openapi/blob/c0bc89c544954ba8d70991f5603479c32f507043/openapi.json#L2975
    private string Adjust(string cultureName)
    {
        cultureName = cultureName.ToUpperInvariant();
        if (cultureName != "EN" && cultureName != "PT")
            return cultureName;

        var adjustedCultureName = cultureName switch
        {
            "EN" => "EN-US",
            "PT" => "PT-PT",
            _ => throw new ArgumentOutOfRangeException(nameof(cultureName), cultureName)
        };
        _messages.Add(Message.Info($"DeepL can't translate to {cultureName}, using {adjustedCultureName} instead"));
        return adjustedCultureName;
    }
}