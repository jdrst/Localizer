using Localizer.Application.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.FileProviders.Physical;

namespace Localizer.Infrastructure.Configuration;

internal class ConfigurationProvider
{
    private readonly IConfigurationRoot _config;

    public ConfigurationProvider(IPathProvider pathProvider)
    {
        var configBuilder = new ConfigurationBuilder();
        using var fileProvider = new PhysicalFileProvider(pathProvider.Root,
            ExclusionFilters.System | ExclusionFilters.Hidden);

        configBuilder
            .SetFileProvider(fileProvider)
            .AddJsonFile(pathProvider.GlobalConfigPath)
            .AddJsonFile(pathProvider.RelativeLocalConfigPath, true);

        _config = configBuilder.Build();
    }
        
    public IConfigurationSection GetSection(string key) => _config.GetSection(key);
    public IConfiguration GetConfig() => _config;
}