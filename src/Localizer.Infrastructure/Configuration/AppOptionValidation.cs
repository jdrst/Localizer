using Microsoft.Extensions.Options;

namespace Localizer.Infrastructure.Configuration;

public class AppOptionValidation : IValidateOptions<AppOptions>
{
    internal const string NoOptionsFoundText = "No configuration found.";
    public ValidateOptionsResult Validate(string? name, AppOptions? options)
    {
        var failures = new List<string>();

        if (options is null)
        {
            return ValidateOptionsResult.Fail(NoOptionsFoundText);
        }
        
        //TODO check TranslationTextProviderType valid?

        return failures.Count > 0 ? ValidateOptionsResult.Fail(failures) : ValidateOptionsResult.Success;
    }
}