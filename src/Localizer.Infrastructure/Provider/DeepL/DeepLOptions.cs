namespace Localizer.Infrastructure.Provider.DeepL;

public class DeepLOptions
{
    internal const string Section = "DeepL";
    public string AuthKey { get; set; } = string.Empty;
    public string? Context { get; set; } = string.Empty;
    public string? SourceLanguage { get; set; } = string.Empty;
}