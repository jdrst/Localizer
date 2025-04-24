using System.Globalization;
using DeepL;
using DeepL.Model;
using Localizer.Application.Abstractions;
using Localizer.Core;
using Localizer.Core.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Localizer.Infrastructure.Provider.DeepL;

public sealed class DeepLTranslationTextProvider : ITranslationTextProvider, IDisposable
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

    public async Task<string> GetTranslationFor(string value, CultureInfo cultureInfo, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(cultureInfo);
        TextResult result;
        try
        {
            result = await _client.TranslateTextAsync(value, _sourceLanguage, cultureInfo.Name,
                _translateOptions, ct);
        }
        catch (DeepLException ex)
        {
            switch (ex)
            {
                case QuotaExceededException quotaExceededException:
                case TooManyRequestsException tooManyRequestsException:
                    _messages.Add(Message.Error($"DeepL API Exception: {ex.Message}{Environment.NewLine}" +
                                                $"Inserting '{ReplaceMeTranslationTextProvider.ReplaceText}' instead of a translation."));
                    return ReplaceMeTranslationTextProvider.ReplaceText;
                default:
                //this might throw. in that case we probably don't want to continue though, so that should be okay.
                    throw;
            }
        }

        _charactersBilled += result.BilledCharacters;
        return result.Text;
    }

    public void Dispose()
    {
        _client.Dispose();
    }
}