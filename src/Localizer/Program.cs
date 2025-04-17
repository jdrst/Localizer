using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;
using Localizer;

var originalFile = args[0];
var translatedFile = args[1];

var originalFileText = File.ReadAllText(originalFile, Encoding.UTF8);
var translatedFileText = File.ReadAllText(translatedFile, Encoding.UTF8);

var original = JsonNode.Parse(originalFileText)!.Root;
var translated = JsonNode.Parse(translatedFileText)!.Root;

var translationTextProvider = new ReplaceMeTranslationTextProvider();

NodeInserter.InsertMissingNodes(original.AsObject(), translated.AsObject(), translationTextProvider);
NodeInserter.InsertMissingNodes(translated.AsObject(), original.AsObject(), translationTextProvider);


var jsonSerializerOptions = new JsonSerializerOptions
{
    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
    WriteIndented = true
};

string translatedJson = translated.ToJsonString(jsonSerializerOptions);
string originalJson = original.ToJsonString(jsonSerializerOptions);

var translatedOutFileName = Path.GetFileNameWithoutExtension(translatedFile);
File.WriteAllText($"{translatedOutFileName}_patched.json", translatedJson);

var originalOutFileName = Path.GetFileNameWithoutExtension(originalFile);
File.WriteAllText($"{originalOutFileName}_patched.json", originalJson);