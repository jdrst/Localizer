using System.Globalization;
using Localizer.Infrastructure.Provider;
using Shouldly;

namespace Localizer.Tests.UnitTests.Infrastructure.Provider;

public class ReplaceMeTranslationProviderTest
{
    [Fact]
    public async Task TestGetTranslationsAsync()
    {
        var provider = new ReplaceMeTranslationProvider();

        var input = "foo bar";
        var result = await provider.GetTranslationsAsync([input], new CultureInfo("en_US"), TestContext.Current.CancellationToken);
        
        provider.UsesConsole.ShouldBeFalse();
        result.ShouldHaveSingleItem();
        result[0].ShouldBe($"{ReplaceMeTranslationProvider.ReplaceText} {input}");
    }
}