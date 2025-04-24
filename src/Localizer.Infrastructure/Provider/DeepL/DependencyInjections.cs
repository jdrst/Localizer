using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Localizer.Infrastructure.Provider.DeepL;

public static class DependencyInjections
{
    public static IServiceCollection AddDeepL(this IServiceCollection services)
    {
        services.AddSingleton(DeepLClientFactory.Create)
            .AddSingleton<IValidateOptions<DeepLOptions>, DeepLOptionValidation>()
            .AddOptions<DeepLOptions>()
            .Configure<Configuration.ConfigurationProvider>((opt, provider) => provider.GetSection(DeepLOptions.Section).Bind(opt));
        return services;
    }
}