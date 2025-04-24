namespace Localizer.Infrastructure.Configuration;

public enum TranslationTextProviderType
{
    ReplaceMe,
    Prompt,
    DeepL,
}
public class AppOptions
{
    public TranslationTextProviderType TranslationProvider { get; set; } =
        TranslationTextProviderType.ReplaceMe;
}