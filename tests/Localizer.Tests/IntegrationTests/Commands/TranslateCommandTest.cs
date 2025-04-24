using Localizer.Application.Commands;
using Shouldly;

namespace Localizer.Tests.IntegrationTests.Commands;

public class TranslateCommandTest : IntegrationTest
{
    [Fact]
    public async Task TestSimple()
    {
        var app = DefaultCommandAppTester();
        
        var paths = LocaleFileProvider.DefaultLocales();

        var result = await app.RunAsync(TranslateCommand.Name, Path.GetFileName(paths.First()));
        
        result.ExitCode.ShouldBe(0);
        result.Output.ShouldContain("Translating to English.");
        var locale = await File.ReadAllTextAsync(paths.First(), TestContext.Current.CancellationToken);
        var localeEn = await File.ReadAllTextAsync(paths.Last(), TestContext.Current.CancellationToken);
        await Verify((locale, localeEn));
    }
}