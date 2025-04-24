using Microsoft.Extensions.Options;

namespace Localizer.Infrastructure.Configuration;

public class AppOptionValidation : IValidateOptions<AppOptions>
{
   
    public ValidateOptionsResult Validate(string? name, AppOptions? options)
    {
        var failures = new List<string>();

        if (options is null)
        {
            return ValidateOptionsResult.Fail("No configuration found.");
        }

        if (options.TranslationTextProviderType is TranslationTextProviderType.DeepL)
        {
            if (string.IsNullOrWhiteSpace(options.DeepL?.AuthKey))
                failures.Add("No DeepL AuthKey provided.");
        }

        return failures.Count > 0 ? ValidateOptionsResult.Fail(failures) : ValidateOptionsResult.Success;
    }
}