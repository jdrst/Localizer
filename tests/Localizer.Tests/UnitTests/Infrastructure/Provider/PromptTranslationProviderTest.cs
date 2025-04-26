using System.Globalization;
using Localizer.Infrastructure.Provider;
using Shouldly;
using Spectre.Console.Testing;

namespace Localizer.Tests.UnitTests.Infrastructure.Provider;

public class PromptTranslationProviderTest
{
    [Fact]
    public async Task TestGetTranslationsAsync()
    {
        using var console = new TestConsole();
        console.Input.PushTextWithEnter("bar foo");
        
        var provider = new PromptTranslationProvider(console);
        

        var result = await provider.GetTranslationsAsync(["foo bar"], new CultureInfo("en_US"), TestContext.Current.CancellationToken);
        
        provider.UsesConsole.ShouldBeTrue();
        result.ShouldHaveSingleItem();
        result[0].ShouldBe("bar foo");
        await Verify(console.Output);
    }
}