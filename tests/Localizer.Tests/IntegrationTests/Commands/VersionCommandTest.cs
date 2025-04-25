using Localizer.Application.Commands;
using Shouldly;

namespace Localizer.Tests.IntegrationTests.Commands;

public class VersionCommandTest : IntegrationTest
{
    [Fact]
    public void TestOutput()
    {
        var app = DefaultCommandAppTester();
        
        var result = app.Run(VersionCommand.Name);
        
        result.ExitCode.ShouldBe(0);
        result.Output.ShouldBe(new AppInfo().Version);
    }
}