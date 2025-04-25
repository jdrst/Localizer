using Localizer.Application.Abstractions;
using Localizer.Infrastructure.Files;
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
        
        using var globalCfgFileProvider = new PhysicalFileProvider(Path.GetDirectoryName(pathProvider.GlobalConfigFilePath)!);
        using var localCfgFileProvider = new PhysicalFileProvider(Path.GetDirectoryName(pathProvider.LocalConfigFilePath)!,
            ExclusionFilters.System | ExclusionFilters.Hidden);
        
        configBuilder
            .SetFileProvider(globalCfgFileProvider)
            .AddJsonFile(globalCfgFileProvider, PathProvider.AppSettingsFile, false, false)
            .AddJsonFile(localCfgFileProvider, PathProvider.LocalConfigFile, true, false);

        _config = configBuilder.Build();
    }
        
    public IConfigurationSection GetSection(string key) => _config.GetSection(key);
    public IConfiguration GetConfig() => _config;
}