using System.Globalization;
using System.Text.Json;
using System.Text.Json.Nodes;
using Localizer.Core.Abstractions;

namespace Localizer.Core;

public static class NodeTranslator
{
    public static async Task TranslateNodesAsync(IList<JsonNode> nodes, ITranslationProvider translationProvider, CultureInfo cultureInfo,
        CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(nodes);
        ArgumentNullException.ThrowIfNull(translationProvider);
        
        var flattenedStringNodes = GetFlattenedStringNodes(nodes);
        var translations = await translationProvider
            .GetTranslationsAsync(flattenedStringNodes.Select(node => node.GetValue<string>()).ToArray(), cultureInfo, ct);

        foreach (var idx in Enumerable.Range(0, flattenedStringNodes.Count))
            flattenedStringNodes[idx].ReplaceWith(translations[idx]);
    }

    private static List<JsonNode> GetFlattenedStringNodes(IList<JsonNode> nodes)
    {
        var result = new List<JsonNode>();
        foreach (var node in nodes)
        {
            {
                if (node.GetValueKind() is JsonValueKind.String)
                {
                    result.Add(node);
                    continue;
                }

                if (node.GetValueKind() is not JsonValueKind.Object)
                    throw new ArgumentOutOfRangeException(nameof(nodes));

                foreach (var (_,child) in node.AsObject())
                {
                    result.AddRange(GetFlattenedStringNodes([child!]));
                }
            }
        }

        return result;
    }
}