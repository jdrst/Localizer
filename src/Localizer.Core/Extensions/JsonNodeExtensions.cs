using System.Globalization;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Localizer.Core.Extensions;

internal static class JsonNodeExtensions
{
    internal static async Task<JsonNode> DeepCloneAndReplaceText(this JsonNode node, Func<string, CultureInfo, CancellationToken, Task<string>> replacementTextFunc, CultureInfo cultureInfo, CancellationToken ct = default) => 
        await node.DeepClone().ReplaceTextWith(replacementTextFunc, cultureInfo, ct);

    private static async Task<JsonNode> ReplaceTextWith(this JsonNode node, Func<string, CultureInfo, CancellationToken, Task<string>> replacementTextFunc, CultureInfo cultureInfo, CancellationToken ct = default)
    {
        if (node.GetValueKind() is JsonValueKind.String)
        {
            node = await replacementTextFunc(node.GetValue<string>(), cultureInfo, ct);
            return node;
        }

        if (node.GetValueKind() is not JsonValueKind.Object) 
            throw new ArgumentOutOfRangeException(nameof(node));
        
        foreach (var (propName, child) in node.AsObject())
            node[propName] = await child!.ReplaceTextWith(replacementTextFunc, cultureInfo, ct);

        return node;
    }
}