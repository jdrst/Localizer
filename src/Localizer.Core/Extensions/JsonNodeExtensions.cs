using System.Globalization;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Localizer.Extensions;

internal static class JsonNodeExtensions
{
    internal static async Task<JsonNode> DeepCloneAndReplaceText(this JsonNode node, Func<string, CultureInfo, Task<string>> replacementTextFunc, CultureInfo cultureInfo) => 
        await node.DeepClone().ReplaceTextWith(replacementTextFunc, cultureInfo);

    private static async Task<JsonNode> ReplaceTextWith(this JsonNode node, Func<string, CultureInfo, Task<string>> replacementTextFunc, CultureInfo cultureInfo)
    {
        if (node.GetValueKind() is JsonValueKind.String)
        {
            node = await replacementTextFunc(node.GetValue<string>(), cultureInfo);
            return node;
        }

        if (node.GetValueKind() is not JsonValueKind.Object) 
            throw new ArgumentOutOfRangeException(nameof(node));
        
        foreach (var (propName, child) in node.AsObject())
            node[propName] = await child!.ReplaceTextWith(replacementTextFunc, cultureInfo);

        return node;
    }
}