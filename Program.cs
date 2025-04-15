using JsonDiffPatchDotNet;
using JsonDiffPatchDotNet.Formatters.JsonPatch;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using MSOperation = Microsoft.AspNetCore.JsonPatch.Operations.Operation;
using Operation = JsonDiffPatchDotNet.Formatters.JsonPatch.Operation;


var originalFile = args[0];
var translatedFile = args[1];



var originalFileText = File.ReadAllText(originalFile);
var translatedFileText = File.ReadAllText(translatedFile);

var original = JToken.Parse(originalFileText);
var translated = JToken.Parse(translatedFileText);

var patchForRight = GetOperations(original, translated).ToList();
var patchForLeft = GetOperations(translated, original).ToList();

if (patchForRight.Any())
{
    Patch(translated, patchForRight);
    WritePatchedFile(translatedFile, translated);
}

if (patchForLeft.Any())
{
    Patch(original, patchForLeft);
    WritePatchedFile(originalFile, original);
}

IEnumerable<MSOperation> GetOperations(JToken original, JToken translated)
{
    var jdp = new JsonDiffPatch(); ;

    var diff = jdp.Diff(translated, original);

    var formatter = new JsonDeltaFormatter();
    var ops = formatter.Format(diff);
    return ops.Where(op => op.GetOperationType() is OperationType.Add)
        .Select(ConvertToMsOperation);
}

static void Patch(JToken json, List<MSOperation> operations)
{
    var patchDoc = new JsonPatchDocument(operations, new DefaultContractResolver());
    patchDoc.ApplyTo(json);
}

static void WritePatchedFile(string oldFileName, JToken json)
{
    var fileName = Path.GetFileNameWithoutExtension(oldFileName);
    File.WriteAllText($"{fileName}_patched.json" ,json.ToString());
}

MSOperation ConvertToMsOperation(Operation op) => new(op.Op, op.Path, op.From, ReplaceValues((JToken)op.Value));

static JToken ReplaceValues(JToken jToken)
{
    if (jToken is JValue value)
    {
        return JValue.CreateString($"<<replaceme>> {value}");
    }

    if (jToken is JObject jObject)
    {
        foreach (var child in jObject)
        {
            jObject[child.Key] = ReplaceValues(child.Value);
        }

        return jObject;
    }

    throw new ArgumentOutOfRangeException(nameof(jToken));
}

static class OperationExtensions
{
    internal static OperationType GetOperationType(this Operation op) => op.Op.ToLowerInvariant() switch
    {
        "add" => OperationType.Add,
        "remove" => OperationType.Remove,
        "replace" => OperationType.Replace,
        "move" => OperationType.Move,
        "copy" => OperationType.Copy,
        "test" => OperationType.Test,
        _ => throw new ArgumentOutOfRangeException(nameof(op))
    };
}