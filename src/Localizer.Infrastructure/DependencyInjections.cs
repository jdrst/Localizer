using Localizer.Application.Abstractions;
using Localizer.Infrastructure.Configuration;
using Localizer.Infrastructure.Files;
using Localizer.Infrastructure.Provider.DeepL;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ConfigurationProvider = Localizer.Infrastructure.Configuration.ConfigurationProvider;

namespace Localizer.Infrastructure;

public static class DependencyInjections
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services
            .AddSingleton(TranslationTextProviderFactory.Create)
            .AddSingleton<IFileHandler, FileHandler>()
            .AddLogging(builder => builder.AddConsole())
            .AddScoped<IConfigValueSetter, ConfigValueSetter>()
            .AddScoped<IConfigValueGetter, ConfigValueGetter>()
            .AddSingleton<IValidateOptions<AppOptions>, AppOptionValidation>()
            .AddSingleton<IPathProvider>(new PathProvider())
            .AddSingleton<ConfigurationProvider>()
            .AddDeepL()
            .AddOptions<AppOptions>()
            .Configure<ConfigurationProvider>((opt, provider) => provider.GetConfig().Bind(opt));
        return services;
    }
}