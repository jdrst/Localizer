using Localizer.Application.Abstractions;
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
            .AddLogging(builder => builder.AddConsole())
            .AddSingleton<IAppInfo, AppInfo>()
            .AddScoped<IConfigValueSetter, ConfigValueSetter>()
            .AddScoped<IConfigValueGetter, ConfigValueGetter>();

    public static IServiceCollection AddConfiguration(this IServiceCollection services)
    {
        var configBuilder = new ConfigurationBuilder();
        using var fileProvider = new PhysicalFileProvider(AppContext.BaseDirectory, ExclusionFilters.System | ExclusionFilters.Hidden);

        var pathProvider = new PathProvider();
        
        configBuilder
            .SetBasePath(AppContext.BaseDirectory)
            .SetFileProvider(fileProvider)
            .AddJsonFile(pathProvider.GlobalConfigPath())
            .AddJsonFile(pathProvider.RelativeLocalConfigPath, true);

        services.Configure<AppOptions>(configBuilder.Build())
            .AddSingleton<IValidateOptions<AppOptions>, AppOptionValidation>()
            .AddSingleton<IPathProvider>(pathProvider);
        return services;
    }
}