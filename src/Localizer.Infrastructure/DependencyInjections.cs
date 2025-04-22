using Localizer.Core.Abstractions;
using Localizer.Infrastructure.Configuration;
using Localizer.Infrastructure.Files;
using Localizer.Infrastructure.Provider;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.FileProviders.Physical;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Localizer.Infrastructure;

public static class DependencyInjections
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        => services.AddSingleton<ITranslationTextProvider, PromptTranslationTextProvider>()
            .AddSingleton<IFileHandler, FileHandler>()
            .AddLogging(builder => builder.AddConsole());

    public static IServiceCollection AddConfiguration(this IServiceCollection services)
    {
        using var config = new ConfigurationManager();
        using var fileProvider = new PhysicalFileProvider(AppContext.BaseDirectory, ExclusionFilters.System | ExclusionFilters.Hidden);
            
        config
        .SetBasePath(AppContext.BaseDirectory)
        .SetFileProvider(fileProvider)
        .AddJsonFile(Path.Join(AppContext.BaseDirectory, Paths.AppSettingsFile))
        .AddJsonFile(
            Path.GetRelativePath(AppContext.BaseDirectory,
                Path.Join(Environment.CurrentDirectory, Paths.LocalConfigFile)), true);

        services.Configure<AppOptions>(config)
            .AddSingleton<IValidateOptions<AppOptions>, AppOptionValidation>()
            .AddSingleton<IConfigurationManager>(config);
        return services;
    }
}