using Localizer.Application.Commands.Config;
using Shouldly;

namespace Localizer.Tests.IntegrationTests.Commands.Config;

public class ListCommandTest : IntegrationTest
{
    [Fact]
    public async Task TestList()
    {
        var app = DefaultCommandAppTester();
        
        var result = await app.RunAsync("config", ListCommand.Name);
        
        result.ExitCode.ShouldBe(0);
        await Verify(result.Output);
    }
}
