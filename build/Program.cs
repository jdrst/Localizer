using Cake.Common;
using Cake.Common.IO;
using Cake.Common.Tools.DotNet;
using Cake.Common.Tools.DotNet.Build;
using Cake.Common.Tools.DotNet.Pack;
using Cake.Common.Tools.DotNet.Test;
using Cake.Common.Tools.ReportGenerator;
using Cake.Core;
using Cake.Core.IO;
using Cake.Frosting;
using Path = System.IO.Path;

namespace Build;

public static class Program
{
    public static int Main(string[] args)
    {
        return new CakeHost()
            .UseContext<BuildContext>()
            .InstallTool(new Uri("dotnet:?package=dotnet-coverage&version=17.14.2"))
            .InstallTool(new Uri("dotnet:?package=DotnetThirdPartyNotices&version=0.3.3"))
            .Run(args);
    }
}

public class BuildContext(ICakeContext context) : FrostingContext(context)
{
    public string MsBuildConfiguration { get; set; } = context.Argument("configuration", "Release");
}

[TaskName("Clean")]
public sealed class CleanTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        context.CleanDirectories($"{Paths.Dirs.Tests}*/bin/{context.MsBuildConfiguration}");
        context.CleanDirectories($"{Paths.Dirs.Src}*/bin/{context.MsBuildConfiguration}");
    }
}

// TODO: via spdx tooling as soon as Microsoft.Sbom.Targets includes license info?
[TaskName("CreateNoticesFile")]
[IsDependentOn(typeof(CleanTask))]
public sealed class CreateNoticesFileTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        var process = context.ProcessRunner.Start(Tools.DotNetThirdPartyNotices, new ProcessSettings
        {
            Arguments = "--output-filename ../../NOTICES.txt",
            WorkingDirectory = Paths.Dirs.Localizer,
        });
        process.WaitForExit();
    }
}

[TaskName("Build")]
[IsDependentOn(typeof(CreateNoticesFileTask))]
public sealed class BuildTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        context.DotNetBuild(Paths.Files.Localizer, new DotNetBuildSettings
        {
            Configuration = context.MsBuildConfiguration
        });
        context.DotNetBuild(Paths.Files.LocalizerTests, new DotNetBuildSettings
        {
            Configuration = context.MsBuildConfiguration
        });
    }
}

[TaskName("Test")]
[IsDependentOn(typeof(BuildTask))]
public sealed class TestTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        context.DotNetTest(Paths.Files.LocalizerSln, new DotNetTestSettings
        {
            Configuration = context.MsBuildConfiguration,
            NoBuild = true,
        });
    }
}

[TaskName("ReportCoverage")]
[IsDependentOn(typeof(BuildTask))]
public sealed class ReportCoverageTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        var coberturaPath = Path.Join(Paths.Dirs.LocalizerTests, "output.cobertura.xml");
        var process = context.ProcessRunner.Start(Tools.DotNetCoverage, new ProcessSettings
        {
            Arguments = "collect -f cobertura dotnet run",
            WorkingDirectory = Paths.Dirs.LocalizerTests,
        });
        process.WaitForExit();
        context.ReportGenerator(new GlobPattern(coberturaPath), Paths.Output.Coverage, new ReportGeneratorSettings()
        {
            ReportTypes = [ReportGeneratorReportType.Html],
            AssemblyFilters = ["+Localizer*", "-Localizer.Tests*"]
        });
        context.DeleteFile(coberturaPath);
    }
}

[TaskName("Pack")]
[IsDependentOn(typeof(TestTask))]
public sealed class PackTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        context.DotNetPack(Paths.Files.Localizer, new DotNetPackSettings
        {
            Configuration = context.MsBuildConfiguration,
            NoBuild = true,
        });
    }
}

[TaskName("Default")]
[IsDependentOn(typeof(PackTask))]
public sealed class DefaultTask : FrostingTask
{
}

internal static class Tools
{
    internal const string DotNetCoverage = "dotnet-coverage";
    internal const string DotNetThirdPartyNotices = "dotnet-thirdpartynotices";
}
internal static class Paths
{
    private static class Names  {
        internal const string Localizer = "Localizer";
        internal const string LocalizerCore = $"{Localizer}.Core";
        internal const string LocalizerApplication = $"{Localizer}.Application";
        internal const string LocalizerInfrastructure = $"{Localizer}.Infrastructure";
        internal const string LocalizerTests = $"{Localizer}.Tests";
    }

    internal static class Output
    {
        internal const string Coverage = "../coverage";
    }

    internal static class Dirs
    {
        internal const string Root = $"../";
        internal static readonly string Src = Path.Join(Root,"src/");
        internal static readonly string Tests = Path.Join(Root,"tests/");
        internal static readonly string Localizer = Path.Join(Src, $"{Names.Localizer}/");
        internal static readonly string LocalizerTests = Path.Join(Tests, $"{Names.LocalizerTests}/");
    }
    internal static class Files  {
        internal static readonly string LocalizerSln = Path.Join(Dirs.Root,$"{Names.Localizer}.sln");
        internal static readonly string Localizer = Path.Join(Dirs.Localizer,$"{Names.Localizer}.csproj");
        internal static readonly string LocalizerTests = Path.Join(Dirs.LocalizerTests,$"{Names.LocalizerTests}.csproj");
    }
}