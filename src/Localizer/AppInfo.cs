using Localizer.Application.Abstractions;

namespace Localizer;

internal class AppInfo : IAppInfo
{
    public string Name() => ThisAssembly.AssemblyName;
    public string Version() => ThisAssembly.AssemblyInformationalVersion;
}