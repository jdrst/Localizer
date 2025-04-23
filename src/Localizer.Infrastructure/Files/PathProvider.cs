using Localizer.Application.Abstractions;

namespace Localizer.Infrastructure.Files;

internal class PathProvider : IPathProvider
{
    private const string AppSettingsFile = "appsettings.json";
    private const string LocalConfigFile = ".localizer";
    
    private readonly string _localConfigPath = Path.Join(Environment.CurrentDirectory, LocalConfigFile);
    private readonly string _globalConfigPath = Path.Join(AppContext.BaseDirectory, AppSettingsFile);

    public string LocalConfigPath() => _localConfigPath;
    public string GlobalConfigPath() => _globalConfigPath;
    internal string RelativeLocalConfigPath => Path.GetRelativePath(AppContext.BaseDirectory, _localConfigPath);
}