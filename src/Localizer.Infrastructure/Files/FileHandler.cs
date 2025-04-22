using System.Globalization;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;
using Localizer.Core;
using Localizer.Core.Abstractions;

namespace Localizer.Infrastructure.Files;

public class FileHandler : IFileHandler
{
    private JsonObject _base = null!;
    private CulturedJson[] _culturedJsons = [];
    private readonly List<Message> _messages = [];
    
    private readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        WriteIndented = true
    };
    
    public CulturedJson[] CulturedJsons() => _culturedJsons;

    public IReadOnlyList<Message> Messages() => _messages.AsReadOnly();

    public JsonObject Base() => _base;
    public async Task<bool> InitializeAsync(string baseFilePath)
    {
        var baseFileName = Path.GetFileNameWithoutExtension(baseFilePath);
        var workingDir = Path.Join(Environment.CurrentDirectory, Path.GetDirectoryName(baseFilePath));
        
        if (!File.Exists(baseFilePath))
        {
            _messages.Add(Message.Error($"File '{baseFilePath}' doesn't exists in '{workingDir}'."));
            return false;
        }
        
        _base = await ReadFileAsync(baseFilePath);
        var searchPattern = $"{baseFileName}_*.json";
        var files = Directory.GetFiles(workingDir, searchPattern);

        if (files.Length < 1)
        {
            _messages.Add(Message.Error($"Couldn't find any translation files with search pattern {searchPattern} in {Environment.CurrentDirectory}."));
            return false;
        }

        var culturedJsons = new CulturedJson[files.Length];

        foreach (var (idx, filePath) in files.Index())
        {
            var cultureString = Path.GetFileNameWithoutExtension(filePath).Split("_").Skip(1).LastOrDefault();
            var cultureInfo = cultureString == null ? CultureInfo.InvariantCulture : new CultureInfo(cultureString);

            var node = await ReadFileAsync(filePath);
            culturedJsons[idx] = new CulturedJson(filePath, node, cultureInfo);
        }

        _culturedJsons = culturedJsons;
        return true;
    }

    public async Task WriteFilesAsync(string? prefix)
    {
        foreach (var (path, json, _) in _culturedJsons)
            await WriteFileAsync(path, prefix, json);
    }

    private static async Task<JsonObject> ReadFileAsync(string path)
    {
        var fileText = await File.ReadAllTextAsync(path, Encoding.UTF8);
        return JsonNode.Parse(fileText)!.Root.AsObject();
    }

    private async Task WriteFileAsync(string path, string? prefix, JsonObject jsonObject)
    {
        string serializedJson = jsonObject.ToJsonString(_jsonSerializerOptions);

        var outPath = string.IsNullOrWhiteSpace(prefix)
            ? path
            : $"{prefix}_{Path.GetFileNameWithoutExtension(path)}.json";
            
        await File.WriteAllTextAsync(outPath, serializedJson);
    }
}