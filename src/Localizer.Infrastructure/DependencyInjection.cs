using Localizer.Abstractions;
using Localizer.Provider;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Localizer;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        => services.AddScoped<ITranslationTextProvider, ReplaceMeTranslationTextProvider>()
            .AddSingleton<IFileHandler, FileHandler>()
            .AddLogging(builder => builder.AddConsole());
}