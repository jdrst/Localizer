using Localizer.Application.Abstractions;

namespace Localizer.Infrastructure.Files;

internal class PathProvider : IPathProvider
{
    internal const string AppSettingsFile = "appsettings.json";
    internal const string LocalConfigFile = ".localizer";

    public string LocalConfigFilePath { get; } = Path.Join(Environment.CurrentDirectory, LocalConfigFile);
    public string GlobalConfigFilePath { get; } = Path.Join(AppContext.BaseDirectory, AppSettingsFile);
}