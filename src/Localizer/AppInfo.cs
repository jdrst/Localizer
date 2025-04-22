using Localizer.Core.Abstractions;

namespace Localizer;

internal class AppInfo : IAppInfo
{
    public static string Name => ThisAssembly.AssemblyName;
    public static string Version => ThisAssembly.AssemblyInformationalVersion;
}