using Microsoft.Extensions.Options;

namespace Localizer.Infrastructure.Provider.DeepL;

public class DeepLOptionValidation : IValidateOptions<DeepLOptions>
{
    internal const string NoAuthKeyProvidedMessage = "DeepL was configured as translator but no DeepL AuthKey is provided.";
   
    public ValidateOptionsResult Validate(string? name, DeepLOptions? options)
    {
        var failures = new List<string>();

        if (options is null)
        {
            return ValidateOptionsResult.Fail("No configuration found.");
        }

        if (string.IsNullOrWhiteSpace(options.AuthKey))
            failures.Add(NoAuthKeyProvidedMessage);

        return failures.Count > 0 ? ValidateOptionsResult.Fail(failures) : ValidateOptionsResult.Success;
    }
}