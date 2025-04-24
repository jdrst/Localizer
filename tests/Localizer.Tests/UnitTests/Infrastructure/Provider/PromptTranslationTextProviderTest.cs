using System.Globalization;
using Localizer.Infrastructure.Provider;
using Shouldly;
using Spectre.Console.Testing;

namespace Localizer.Tests.UnitTests.Infrastructure.Provider;

public class PromptTranslationTextProviderTest
{
    [Fact]
    public async Task TestGetTranslationFor()
    {
        using var console = new TestConsole();
        console.Input.PushTextWithEnter("bar foo");
        
        var provider = new PromptTranslationTextProvider(console);
        

        var result = await provider.GetTranslationFor("foo bar", new CultureInfo("en_US"), TestContext.Current.CancellationToken);
        
        provider.UsesConsole().ShouldBeTrue();
        result.ShouldBe("bar foo");
        await Verify(console.Output);
    }
}