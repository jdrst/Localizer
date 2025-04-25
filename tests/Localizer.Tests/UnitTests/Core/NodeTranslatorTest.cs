using System.Globalization;
using System.Text.Json.Nodes;
using Localizer.Core;
using Localizer.Infrastructure.Provider;

namespace Localizer.Tests.UnitTests.Core;

public class NodeTranslatorTest
{
    [Fact]
    public async Task TestTranslateNodes()
    {
        var node = JsonNode.Parse(TestData.Json.DefaultLocale)!;
        await NodeTranslator.TranslateNodesAsync([node], new ReplaceMeTranslationTextProvider(), new CultureInfo("en-US"), TestContext.Current.CancellationToken);
        
        Assert.True(true);
    }
}