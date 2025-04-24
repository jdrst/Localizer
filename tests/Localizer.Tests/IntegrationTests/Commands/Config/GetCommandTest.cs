using Localizer.Application.Commands.Config;
using Localizer.Infrastructure.Provider.DeepL;
using Shouldly;

namespace Localizer.Tests.IntegrationTests.Commands.Config;

public class GetCommandTest : IntegrationTest
{
    [Fact]
    public async Task TestGet()
    {
        var app = DefaultCommandAppTester();
        
        var result = await app.RunAsync("config", GetCommand.Name, $"{DeepLOptions.Section}:{nameof(DeepLOptions.Context)}");
        
        result.ExitCode.ShouldBe(0);
        await Verify(result.Output);
    }
}