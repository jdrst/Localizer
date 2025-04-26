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
using Path = Cake.Core.IO.Path;

namespace Build;

public static class Program
{
    public static int Main(string[] args)
    {
        return new CakeHost()
            .UseContext<BuildContext>()
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

[TaskName("ToolRestore")]
[IsDependentOn(typeof(CleanTask))]
public sealed class ToolRestoreTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context) 
        => context.DotNetTool(Paths.Files.LocalizerSln, "tool", "restore");
}

// TODO: via spdx tooling as soon as Microsoft.Sbom.Targets includes license info?
[TaskName("CreateNoticesFile")]
[IsDependentOn(typeof(ToolRestoreTask))]
public sealed class CreateNoticesFileTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context) 
        => context.DotNetTool(Paths.Files.LocalizerCsproj, Tools.DotNetThirdPartyNotices, "--output-filename ../../NOTICES.txt");
}

[TaskName("Build")]
[IsDependentOn(typeof(ToolRestoreTask))]
public sealed class BuildTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        context.DotNetBuild(Paths.Files.LocalizerCsproj.ToString(), new DotNetBuildSettings
        {
            Configuration = context.MsBuildConfiguration
        });
        context.DotNetBuild(Paths.Files.LocalizerTestsCsproj.ToString(), new DotNetBuildSettings
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
        context.DotNetTest(Paths.Files.LocalizerSln.ToString(), new DotNetTestSettings
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
        var coberturaPath = Paths.Dirs.LocalizerTests.CombineWithFilePath("output.cobertura.xml");
        context.DotNetTool(Paths.Files.LocalizerTestsCsproj, Tools.DotNetCoverage, "collect -f cobertura dotnet run");
        context.ReportGenerator(coberturaPath, Paths.Output.Coverage, new ReportGeneratorSettings()
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
        context.DotNetPack(Paths.Files.LocalizerCsproj.ToString(), new DotNetPackSettings
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
        internal static readonly DirectoryPath Coverage = "../coverage";
    }

    internal static class Dirs
    {
        internal static readonly DirectoryPath Root = "../";
        internal static readonly DirectoryPath Src = Root.Combine("src/");
        internal static readonly DirectoryPath Tests = Root.Combine("tests/");
        internal static readonly DirectoryPath Localizer = Src.Combine($"{Names.Localizer}/");
        internal static readonly DirectoryPath LocalizerTests = Tests.Combine($"{Names.LocalizerTests}/");
    }
    internal static class Files  {
        internal static readonly FilePath LocalizerSln = Dirs.Root.GetFilePath($"{Names.Localizer}.sln");
        internal static readonly FilePath LocalizerCsproj = Dirs.Localizer.GetFilePath($"{Names.Localizer}.csproj");
        internal static readonly FilePath LocalizerTestsCsproj = Dirs.LocalizerTests.GetFilePath($"{Names.LocalizerTests}.csproj");
    }
}