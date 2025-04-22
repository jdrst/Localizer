using System.Globalization;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Localizer.Extensions;

internal static class JsonNodeExtensions
{
    internal static JsonNode DeepCloneAndReplaceText(this JsonNode node, Func<string, CultureInfo, string> replacementTextFunc, CultureInfo cultureInfo) => 
        node.DeepClone().ReplaceTextWith(replacementTextFunc, cultureInfo);

    private static JsonNode ReplaceTextWith(this JsonNode node, Func<string, CultureInfo, string> replacementTextFunc, CultureInfo cultureInfo)
    {
        if (node.GetValueKind() is JsonValueKind.String)
        {
            node = replacementTextFunc(node.GetValue<string>(), cultureInfo);
            return node;
        }

        if (node.GetValueKind() is not JsonValueKind.Object) 
            throw new ArgumentOutOfRangeException(nameof(node));
        
        foreach (var (propName, child) in node.AsObject())
            node[propName] = child!.ReplaceTextWith(replacementTextFunc, cultureInfo);

        return node;
    }
}