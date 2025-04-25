using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;
using Localizer.Core;
using Shouldly;

namespace Localizer.Tests.UnitTests.Core;

public class NodeInserterTest
{
    [Fact]
    public void TestString()
    {
        var from = JsonNode.Parse("""{ "hello": "text" }""")!.AsObject();
        var to = JsonNode.Parse("{}")!.AsObject();

        var (nodes, messages) = NodeInserter.InsertMissingNodes(from, to);

        to["hello"].ShouldNotBeNull();
        nodes[0].ShouldBe(to["hello"]);
    }
    
    [Fact]
    public void TestObject()
    {
        var from = JsonNode.Parse("""{ "foo": {"some": "text"} }""")!.AsObject();
        var to = JsonNode.Parse("{}")!.AsObject();

        var (nodes, messages) = NodeInserter.InsertMissingNodes(from, to);
        
        to["foo"]!["some"]!.ShouldNotBeNull();
        nodes[0].ShouldBe(to["foo"]);
    }

    [Fact]
    public async Task TestComplex()
    {
        var from = JsonNode.Parse(TestData.Json.DefaultLocale)!.AsObject();
        var to = JsonNode.Parse(TestData.Json.DefaultLocaleEn)!.AsObject();
        
        _ = NodeInserter.InsertMissingNodes(from, to);

        var jsonSerializerOptions = new JsonSerializerOptions{WriteIndented = true, Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,};
        await Verify(to.ToJsonString(jsonSerializerOptions));
    }
    
    [Fact]
    public async Task TestConflicting()
    {
        var from = JsonNode.Parse("""{ "foo": {"some": "text"} }""")!.AsObject();
        var to = JsonNode.Parse("""{ "foo": "some text" }""")!.AsObject();
        
        var (_, messages) = NodeInserter.InsertMissingNodes(from, to);

        var jsonSerializerOptions = new JsonSerializerOptions{WriteIndented = true, Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,};
        await Verify((to.ToJsonString(jsonSerializerOptions), messages));
    }
}
