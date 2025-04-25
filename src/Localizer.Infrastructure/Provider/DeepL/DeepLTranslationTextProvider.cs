using System.Globalization;
using DeepL;
using Localizer.Application.Abstractions;
using Localizer.Core;
using Localizer.Core.Abstractions;
using Microsoft.Extensions.Options;

namespace Localizer.Infrastructure.Provider.DeepL;

internal class DeepLTranslationTextProvider : ITranslationTextProvider
{
    private const string CharactersBilled = "Characters billed this session: ";
    public IReadOnlyList<Message> Messages
    {
        get
        {
            //TODO: maybe not in the getter..
            if (!_messages.Any(msg => msg.Text.StartsWith(CharactersBilled, StringComparison.InvariantCulture)))
                    _messages.Add(Message.Info($"{CharactersBilled}{_charactersBilled}"));
            return _messages;
        }
    }

    public bool UsesConsole => false;
    public async Task<string[]> GetTranslationsAsync(string[] texts, CultureInfo cultureInfo, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(cultureInfo);
        ArgumentNullException.ThrowIfNull(texts);
        
        var targetLanguage = ToDeepLLanguage(cultureInfo);
        
        if (targetLanguage is null)
            return await FallbackAsync($"DeepL can't translate to {cultureInfo.Name}", texts, cultureInfo, ct);

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
            switch (ex)
            {
                case QuotaExceededException:
                case TooManyRequestsException:
                    return await FallbackAsync($"DeepL API Exception: {ex.Message}{Environment.NewLine}", texts, cultureInfo, ct);
                //this might throw. in that case we probably don't want to continue though, so that should be okay.
                default: throw;
            }
        }
    }

    private int _charactersBilled;
    private readonly ITranslator _client;
    private readonly TextTranslateOptions _translateOptions;
    private readonly string? _sourceLanguage;
    private readonly List<Message> _messages = [];


    public DeepLTranslationTextProvider(IOptions<DeepLOptions> options, ITranslator deepLClient, IAppInfo appInfo)
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

    private async Task<string[]> FallbackAsync(string message, string[] texts, CultureInfo cultureInfo, CancellationToken ct = default)
    {
        var replaceMeProvider = new ReplaceMeTranslationTextProvider();
        _messages.Add(Message.Error($"{message}. Inserting '{ReplaceMeTranslationTextProvider.ReplaceText}' instead of a translation."));
        return await replaceMeProvider.GetTranslationsAsync(texts, cultureInfo, ct);
    }

    //taken from https://github.com/DeepLcom/openapi/blob/c0bc89c544954ba8d70991f5603479c32f507043/openapi.json#L2975
    private static readonly string[] SimpleDeepLLanguages =
    [
        "BG", "CS", "DA", "DE", "EL", "ES", "ET", "FI", "FR", "HU", "ID", "IT", "JA", "KO", "LT",
        "LV", "NB", "NL", "PL", "RO", "RU", "SK", "SL", "SV", "TR", "UK", "ZH", 
    ];

    private static readonly string[] RegionalDeepLLanguages = ["PT-BR", "PT-PT", "ZH-HANS", "EN-GB", "EN-US"];
    private string? ToDeepLLanguage(CultureInfo cultureInfo)
    {
        var cultureName = cultureInfo.Name.ToUpperInvariant();
        if (cultureName.Contains('-', StringComparison.InvariantCulture))
        {
            if (RegionalDeepLLanguages.Contains(cultureName))
                return cultureName;
            cultureName = cultureName.Split("-", StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
        }
        if (SimpleDeepLLanguages.Contains(cultureName))
            return cultureName;
        //outliers: PT, EN
        if (TryGetOutliers(cultureName, out var newCultureName))
        {
            _messages.Add(Message.Info($"DeepL can't translate to {cultureName}, using {newCultureName} instead"));
            return newCultureName;
        }
        return null;
    }

    private static bool TryGetOutliers(string? cultureName, out string? newCultureName)
    {
        newCultureName = null;
        if (cultureName != "EN" && cultureName != "PT")
            return false;

        newCultureName = cultureName switch
        {
            "EN" => "EN-US",
            "PT" => "PT-PT",
            _ => throw new ArgumentOutOfRangeException(nameof(cultureName), cultureName)
        };
        return true;
    }
}