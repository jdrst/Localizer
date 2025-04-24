using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Localizer.Infrastructure;

public static class JsonHelper
{
    private static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        WriteIndented = true, 
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };
    public static async Task<JsonNode?> LoadAsync(string path)
    {
        if (!File.Exists(path))
            return null;
        await using var reader = File.OpenRead(path);
        return await JsonNode.ParseAsync(reader); 
    }
    
    public static async Task WriteAsync(this JsonNode node, string path)
    {
        ArgumentNullException.ThrowIfNull(node, nameof(node));
        
        var json = node.ToJsonString(JsonSerializerOptions);
        await File.WriteAllTextAsync(path, json);
    }

    public static async Task<IDictionary<string, string>> GetOptionsFrom(string filePath)
    {
        var node = await LoadAsync(filePath) ?? new JsonObject();

        var dict = new Dictionary<string, string>();
        return dict.AddNodes(node);
    }

    static IDictionary<string, string> AddNodes(this IDictionary<string, string> dict, JsonNode node, string currentKey = "")
    {
        if (node.Parent is not null)
            currentKey = string.IsNullOrWhiteSpace(currentKey) ? node.GetPropertyName() : string.Join(':', currentKey, node.GetPropertyName());
        if (node is JsonValue val)
            dict[currentKey] = val.GetValue<string>();
        if (node is JsonObject obj)
            foreach (var (_, child) in obj.AsObject())
                dict.AddNodes(child!, currentKey);

        return dict;
    }
}