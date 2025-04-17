using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;
using Shouldly;

namespace Localizer.Tests;

public class NodeInserterTest
{
    [Fact]
    public void TestString()
    {
        var from = JsonNode.Parse("{ \"hello\": \"text\" }")!.AsObject();
        var to = JsonNode.Parse("{}")!.AsObject();

        NodeInserter.InsertMissingNodes(from, to, new ReplaceMeTranslationTextProvider());
        
        to["hello"]!.GetValue<string>().ShouldBe("<<replaceme>> text");
    }
    
    [Fact]
    public void TestObject()
    {
        var from = JsonNode.Parse("{ \"foo\": {\"some\": \"text\"} }")!.AsObject();
        var to = JsonNode.Parse("{}")!.AsObject();

        NodeInserter.InsertMissingNodes(from, to, new ReplaceMeTranslationTextProvider());
        
        to["foo"]!["some"]!.GetValue<string>().ShouldBe("<<replaceme>> text");
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
        
        NodeInserter.InsertMissingNodes(from, to, new ReplaceMeTranslationTextProvider());
        NodeInserter.InsertMissingNodes(to, from, new ReplaceMeTranslationTextProvider());

        var jsonSerializerOptions = new JsonSerializerOptions{WriteIndented = true, Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,};
        await Verify($"""
                      From:
                      {from.ToJsonString(jsonSerializerOptions)}
                      To:
                      {to.ToJsonString(jsonSerializerOptions)}
                      """);
    }
}
