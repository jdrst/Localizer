using DeepL;
using Localizer.Application.Abstractions;
using Localizer.Core.Abstractions;
using Localizer.Infrastructure.Configuration;
using Localizer.Infrastructure.Files;
using Localizer.Infrastructure.Provider;
using Localizer.Infrastructure.Provider.DeepL;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.FileProviders.Physical;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Spectre.Console;

namespace Localizer.Infrastructure;

public static class DependencyInjections
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        => services.AddSingleton(TranslationTextProviderFactory.Create)
            .AddSingleton<IFileHandler, FileHandler>()
            .AddLogging(builder => builder.AddConsole())
            .AddScoped<IConfigValueSetter, ConfigValueSetter>()
            .AddScoped<IConfigValueGetter, ConfigValueGetter>()
            .AddSingleton<ITranslator>(sp =>
                DeepLClientFactory.Create(sp.GetRequiredService<IOptions<DeepLOptions>>(), sp.GetRequiredService<IAppInfo>())
                );

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

public static class TranslationTextProviderFactory
{
    public static ITranslationTextProvider Create(IServiceProvider services)
    {
        ArgumentNullException.ThrowIfNull(services);
        
        var options = services.GetRequiredService<IOptions<AppOptions>>();

        return options.Value.TranslationTextProviderType switch
        {
            TranslationTextProviderType.ReplaceMe => new ReplaceMeTranslationTextProvider(),
            TranslationTextProviderType.Prompt => new PromptTranslationTextProvider(
                services.GetRequiredService<IAnsiConsole>()),
            TranslationTextProviderType.DeepL => new DeepLTranslationTextProvider(
                services.GetRequiredService<ILogger<DeepLTranslationTextProvider>>(),
                services.GetRequiredService<IOptions<DeepLOptions>>(), services.GetRequiredService<ITranslator>(),
                services.GetRequiredService<IAppInfo>()),
            _ => throw new ArgumentOutOfRangeException(nameof(services))
        };
    }
}