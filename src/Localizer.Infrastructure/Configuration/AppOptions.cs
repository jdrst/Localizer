using Localizer.Infrastructure.Provider.DeepL;

namespace Localizer.Infrastructure.Configuration;

public enum TranslationTextProviderType
{
    ReplaceMe,
    Prompt,
    DeepL,
}
public class AppOptions
{
    public TranslationTextProviderType TranslationTextProviderType { get; set; } =
        TranslationTextProviderType.ReplaceMe;

    public DeepLOptions? DeepL { get; set; }
}