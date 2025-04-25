using Localizer.Application.Abstractions;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Localizer.Application.Commands;

internal class VersionCommand(IAnsiConsole console, IAppInfo appInfo) : Command
{
    public const string Name = "version";
    public const string Description = "Prints version information."; 
    public const string Example = "version";

    public override int Execute(CommandContext ctx)
    {
        console.WriteLine(appInfo.Version);
        return 0;
    }
}