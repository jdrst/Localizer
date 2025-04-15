using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Unicode;

var originalFile = args[0];
var translatedFile = args[1];

var originalFileText = File.ReadAllText(originalFile, Encoding.UTF8);
var translatedFileText = File.ReadAllText(translatedFile, Encoding.UTF8);

var original = JsonNode.Parse(originalFileText)!.Root;
var translated = JsonNode.Parse(translatedFileText)!.Root;


var encoderSettings = new TextEncoderSettings();

var stream = new MemoryStream();
var jsonWriter = new Utf8JsonWriter(stream, new JsonWriterOptions
{
    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
    Indented = true,
});
WalkAndWrite(original, translated, jsonWriter);

jsonWriter.Flush();

string json = Encoding.UTF8.GetString(stream.ToArray());

var outFileName = Path.GetFileNameWithoutExtension(translatedFile);
File.WriteAllText($"{outFileName}_patched.json", json);


static void WalkAndWrite(JsonNode original, JsonNode translated, Utf8JsonWriter writer)
{
    var valueKind = original.GetValueKind();
    if (valueKind is JsonValueKind.String)
    {
        if (translated.TryGetPath(original.GetPath().Substring(2), out var node))
        {
            writer.WritePropertyName(node!.GetPropertyName());
            node.WriteTo(writer);
        }
        else
        {
            writer.WritePropertyName(original.GetPropertyName());
            writer.WriteStringValue(original.ReplaceMeValue());
        }
        return;
    } 
    if (valueKind is JsonValueKind.Object)
    {
        if (original.Parent is not null) 
            writer.WritePropertyName(original.GetPropertyName());
        writer.WriteStartObject();
        foreach (var (_, child) in original.AsObject())
        {
            WalkAndWrite(child, translated, writer);
        }
        writer.WriteEndObject();
        return;
    }

    throw new ArgumentOutOfRangeException(nameof(original));
}

static class Extensions
{
    internal static string ReplaceMeValue(this JsonNode node) => $"<<replaceme>> {node.GetValue<string>()}";
    internal static bool TryGetPath(this JsonNode? node, string path, out JsonNode? result)
    {
        result = null;
        if (node is null)
            return false;
    
        var dotIndex = path.IndexOf('.');
        if (dotIndex == -1)
        {
            result = node[path];
            return node[path] is not null;
        }

        return TryGetPath(node[path.Substring(0, dotIndex)], path.Substring(dotIndex+1), out result);
    } 
}


