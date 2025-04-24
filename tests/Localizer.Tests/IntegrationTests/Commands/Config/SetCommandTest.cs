using Localizer.Application.Commands.Config;
using Shouldly;

namespace Localizer.Tests.IntegrationTests.Commands.Config;

public class SetCommandTest : IntegrationTest
{
    [Theory]
    [InlineData(TestData.Config.Keys.DeepL_Context, "foo", false)] //set local
    [InlineData(TestData.Config.Keys.DeepL_Context, "bar", true)]  //set global
    [InlineData(TestData.Config.Keys.DeepL_Context, "", true)]     //unset global

    public async Task TestSet(string key, string value, bool isGlobal)
    {
        var app = DefaultCommandAppTester();

        List<string> args = ["config", SetCommand.Name, key, value];
        if (isGlobal)
            args.Add("-g");
        
        var result = await app.RunAsync(args.ToArray());
        
        result.ExitCode.ShouldBe(0);
        await Verify((result.Output, await File.ReadAllTextAsync(TestPathProvider!.GlobalConfigPath, TestContext.Current.CancellationToken), await File.ReadAllTextAsync(TestPathProvider.LocalConfigPath, TestContext.Current.CancellationToken)));
    }
}