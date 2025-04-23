using Localizer.Application.Abstractions;

namespace Localizer.Infrastructure;

internal class AppInfo : IAppInfo
{
    public static string Name => ThisAssembly.AssemblyName;
    public static string Version => ThisAssembly.AssemblyInformationalVersion;
}