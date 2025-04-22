using System.Globalization;
using System.Text.Json;
using System.Text.Json.Nodes;
using Localizer.Core.Abstractions;
using Localizer.Core.Extensions;

namespace Localizer.Core;

public static class NodeInserter
{
    public static async Task InsertMissingNodes(JsonObject from, JsonObject to, ITranslationTextProvider translationTextProvider, CultureInfo cultureInfo)
    {
        ArgumentNullException.ThrowIfNull(translationTextProvider);
        ArgumentNullException.ThrowIfNull(from);
        ArgumentNullException.ThrowIfNull(to);
        
        foreach (var (propName, fromChild) in from)
        {
            var toChild = to[propName];
            if (toChild is null)
            {
                    to.Insert(from.IndexOf(propName), propName,
                        await fromChild!.DeepCloneAndReplaceText(translationTextProvider.GetTranslationFor,
                            cultureInfo));
                continue;
            }

            var childKind = fromChild!.GetValueKind();
            var otherKind = toChild.GetValueKind();

            if (childKind is JsonValueKind.String && otherKind is JsonValueKind.String ||
                // we only hande objects and string. just ignore the rest (as the source generator does)
                childKind is not (JsonValueKind.Object or JsonValueKind.String) ||
                otherKind is not (JsonValueKind.Object or JsonValueKind.String))
                continue;


            if (childKind is JsonValueKind.Object && otherKind is JsonValueKind.Object)
            {
                await InsertMissingNodes(fromChild.AsObject(), toChild.AsObject(), translationTextProvider, cultureInfo);
                continue;
            }


            // we are dropping information here but if there's nodes with the same propertyName and different valueKinds we have to die one death.
            // from is the winner in this case
            // TODO: log this as warning!
            to.Remove(propName);
            to.Insert(from.IndexOf(propName), propName,
                await fromChild.DeepCloneAndReplaceText(translationTextProvider.GetTranslationFor,
                    cultureInfo));
        }
    }
}