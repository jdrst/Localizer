using DeepL;
using Localizer.Application.Abstractions;
using Localizer.Core.Abstractions;
using Localizer.Infrastructure.Configuration;
using Localizer.Infrastructure.Provider;
using Localizer.Infrastructure.Provider.DeepL;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Spectre.Console;

namespace Localizer.Infrastructure;

internal static class TranslationTextProviderFactory
{
    public static ITranslationProvider Create(IServiceProvider services)
    {
        ArgumentNullException.ThrowIfNull(services);
        
        var options = services.GetRequiredService<IOptions<AppOptions>>();

        return options.Value.TranslationProvider switch
        {
            TranslationTextProviderType.ReplaceMe => new ReplaceMeTranslationProvider(),
            TranslationTextProviderType.Prompt => new PromptTranslationProvider(
                services.GetRequiredService<IAnsiConsole>()),
            TranslationTextProviderType.DeepL => new DeepLTranslationProvider(
                services.GetRequiredService<IOptions<DeepLOptions>>(), services.GetRequiredService<ITranslator>(),
                services.GetRequiredService<IAppInfo>()),
            _ => throw new ArgumentOutOfRangeException(nameof(services))
        };
    }
}