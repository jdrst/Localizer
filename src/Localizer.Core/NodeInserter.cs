using System.Text.Json;
using System.Text.Json.Nodes;

namespace Localizer.Core;

public static class NodeInserter
{
    public static (List<JsonNode> InsertedNodes, IReadOnlyList<Message> Messages) InsertMissingNodes(JsonObject from, JsonObject to)
    {
        ArgumentNullException.ThrowIfNull(from);
        ArgumentNullException.ThrowIfNull(to);
        
        var nodes = new List<JsonNode>();
        var messages = new List<Message>();
        
        foreach (var (propName, fromChild) in from)
        {
            var toChild = to[propName];
            if (toChild is null)
            {
                var insertedNode = fromChild!.DeepClone();
                to.Insert(from.IndexOf(propName), propName, insertedNode);
                nodes.Add(insertedNode);
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
                var (foo, bar) = InsertMissingNodes(fromChild.AsObject(), toChild.AsObject());
                nodes.AddRange(foo);
                messages.AddRange(bar);
                continue;
            }


            // we are dropping information here but if there's nodes with the same propertyName and different valueKinds we have to die one death.
            // from is the winner in this case
            messages.Add(Message.Info($"Found nodes with the same property name but different kinds. Overriding with the node from the base file."));
            to.Remove(propName);
            var newNode = fromChild.DeepClone();
            to.Insert(from.IndexOf(propName), propName, newNode);
            nodes.Add(newNode);
        }

        return (nodes, messages);
    }
}