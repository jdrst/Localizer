using System.Globalization;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;
using Localizer.Core;
using Localizer.Infrastructure.Provider;
using Shouldly;

namespace Localizer.Tests.Core;

public class NodeInserterTest
{
    [Fact]
    public async Task TestString()
    {
        var from = JsonNode.Parse("{ \"hello\": \"text\" }")!.AsObject();
        var to = JsonNode.Parse("{}")!.AsObject();

        await NodeInserter.InsertMissingNodes(from, to, new ReplaceMeTranslationTextProvider(), CultureInfo.InvariantCulture, TestContext.Current.CancellationToken);
        
        to["hello"]!.GetValue<string>().ShouldBe($"{ReplaceMeTranslationTextProvider.ReplaceText} text");
    }
    
    [Fact]
    public async Task TestObject()
    {
        var from = JsonNode.Parse("{ \"foo\": {\"some\": \"text\"} }")!.AsObject();
        var to = JsonNode.Parse("{}")!.AsObject();

        await NodeInserter.InsertMissingNodes(from, to, new ReplaceMeTranslationTextProvider(), CultureInfo.InvariantCulture, TestContext.Current.CancellationToken);
        
        to["foo"]!["some"]!.GetValue<string>().ShouldBe($"{ReplaceMeTranslationTextProvider.ReplaceText} text");
    }

    [Fact]
    public async Task TestComplex()
    {
        var from = JsonNode.Parse("""
                                  {
                                    "TestKey": "Neutral",
                                    "OnlyHere": "Nür hiär",
                                    "SpecialChars": "\" \\ @ ß \r\n {0}",
                                    "Sub": {
                                      "SubText": "Abc",
                                      "SubText2": "Xyz"
                                    },
                                    "DoublyNested": {
                                      "Something": "Something",
                                      "Found": "found"
                                    }
                                  }
                                  """)!.AsObject();
        var to = JsonNode.Parse("""
                                {
                                  "TestKey": "Neutral",
                                  "Sub": {
                                    "SubText": "Abc"
                                  },
                                  "NotInOther": "Foo",
                                  "ObjectNotInOther": {
                                    "SubValue": "value"
                                  },
                                  "DoublyNested": {
                                    "Found": "found"
                                  }
                                }
                                """)!.AsObject();
        
        await NodeInserter.InsertMissingNodes(from, to, new ReplaceMeTranslationTextProvider(), CultureInfo.InvariantCulture, TestContext.Current.CancellationToken);

        var jsonSerializerOptions = new JsonSerializerOptions{WriteIndented = true, Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,};
        await Verify(to.ToJsonString(jsonSerializerOptions));
    }
}
