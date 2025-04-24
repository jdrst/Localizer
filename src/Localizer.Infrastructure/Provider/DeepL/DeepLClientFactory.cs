using DeepL;
using Localizer.Application.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Localizer.Infrastructure.Provider.DeepL;

internal static class DeepLClientFactory
{
    internal static ITranslator Create(IServiceProvider services)
    {
        ArgumentNullException.ThrowIfNull(services);
        
        var options = services.GetRequiredService<IOptions<DeepLOptions>>();
        var appInfo = services.GetRequiredService<IAppInfo>();
        return new DeepLClient(options.Value.AuthKey, new DeepLClientOptions()
        {
            appInfo = new AppInfo
            {
                AppName = appInfo.Name,
                AppVersion = appInfo.Version
            }
        });
    }
}