using Localizer.Application.Abstractions;

namespace Localizer.Infrastructure.Files;

internal class PathProvider : IPathProvider
{
    internal const string AppSettingsFile = "appsettings.json";
    internal const string LocalConfigFile = ".localizer";
    
    private readonly string _localConfigPath = Path.Join(Environment.CurrentDirectory, LocalConfigFile);
    private readonly string _globalConfigPath = Path.Join(BaseDir, AppSettingsFile);

    private static readonly string BaseDir = AppContext.BaseDirectory;
    public string Root => BaseDir;
    public string LocalConfigPath => _localConfigPath;
    public string GlobalConfigPath => _globalConfigPath;
    public string RelativeLocalConfigPath => Path.GetRelativePath(BaseDir, _localConfigPath);
}