using DeepL;
using Localizer.Application.Abstractions;
using Localizer.Core.Abstractions;
using Localizer.Infrastructure.Configuration;
using Localizer.Infrastructure.Provider;
using Localizer.Infrastructure.Provider.DeepL;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Spectre.Console;

namespace Localizer.Infrastructure;

internal static class TranslationTextProviderFactory
{
    public static ITranslationTextProvider Create(IServiceProvider services)
    {
        ArgumentNullException.ThrowIfNull(services);
        
        var options = services.GetRequiredService<IOptions<AppOptions>>();

        return options.Value.TranslationProvider switch
        {
            TranslationTextProviderType.ReplaceMe => new ReplaceMeTranslationTextProvider(),
            TranslationTextProviderType.Prompt => new PromptTranslationTextProvider(
                services.GetRequiredService<IAnsiConsole>()),
            TranslationTextProviderType.DeepL => new DeepLTranslationTextProvider(
                services.GetRequiredService<IOptions<DeepLOptions>>(), services.GetRequiredService<ITranslator>(),
                services.GetRequiredService<IAppInfo>()),
            _ => throw new ArgumentOutOfRangeException(nameof(services))
        };
    }
}