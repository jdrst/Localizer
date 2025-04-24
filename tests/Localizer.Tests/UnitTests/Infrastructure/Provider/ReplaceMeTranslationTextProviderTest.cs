using System.Globalization;
using Localizer.Infrastructure.Provider;
using Shouldly;

namespace Localizer.Tests.UnitTests.Infrastructure.Provider;

public class ReplaceMeTranslationTextProviderTest
{
    [Fact]
    public async Task TestGetTranslationFor()
    {
        var provider = new ReplaceMeTranslationTextProvider();

        var input = "foo bar";
        var result = await provider.GetTranslationFor(input, new CultureInfo("en_US"), TestContext.Current.CancellationToken);
        
        provider.UsesConsole().ShouldBeFalse();
        result.ShouldBe($"{ReplaceMeTranslationTextProvider.ReplaceText} {input}");
    }
}