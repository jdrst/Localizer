using System.Globalization;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;
using Localizer.Abstractions;
using Microsoft.Extensions.Logging;
using Localizer.Logging;

namespace Localizer;

public class FileHandler(ILogger<FileHandler> _logger) : IFileHandler
{
    private JsonObject _base = null!;
    private CulturedJson[] _culturedJsons = [];
    private List<Error> _errors = [];
    
    private JsonSerializerOptions _jsonSerializerOptions = new()
    {
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        WriteIndented = true
    };
    
    public CulturedJson[] CulturedJsons() => _culturedJsons;

    public IReadOnlyList<Error> Errors() => _errors.AsReadOnly();

    public JsonObject Base() => _base;
    public async Task<bool> InitializeAsync(string baseFilePath)
    {
        var baseFileName = Path.GetFileNameWithoutExtension(baseFilePath);
        var workingDir = Path.Join(Environment.CurrentDirectory, Path.GetDirectoryName(baseFilePath));
        
        if (!File.Exists(baseFilePath))
        {
            _errors.Add(new($"File '{baseFilePath}' doesn't exists in '{workingDir}'."));
            _logger.FileDoesntExist(baseFilePath);
            return false;
        }
        
        _base = await ReadFileAsync(baseFilePath);
        var searchPattern = $"{baseFileName}_*.json";
        var files = Directory.GetFiles(workingDir, searchPattern);

        if (files.Length < 1)
        {
            _errors.Add(new($"Couldn't find any translation files with search pattern {searchPattern} in {Environment.CurrentDirectory}."));
            _logger.CouldNotFindLocales(baseFilePath);
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

    public async Task WriteFilesAsync(string prefix)
    {
        foreach (var (path, json, _) in _culturedJsons)
            await WriteFileAsync(path, prefix, json);
    }

    private async Task<JsonObject> ReadFileAsync(string path)
    {
        var fileText = await File.ReadAllTextAsync(path, Encoding.UTF8);
        return JsonNode.Parse(fileText)!.Root.AsObject();
    }

    private async Task WriteFileAsync(string path, string prefix, JsonObject jsonObject)
    {
        string serializedJson = jsonObject.ToJsonString(_jsonSerializerOptions);

        var outPath = string.IsNullOrWhiteSpace(prefix)
            ? path
            : $"{prefix}_{Path.GetFileNameWithoutExtension(path)}.json";
            
        await File.WriteAllTextAsync(outPath, serializedJson);
    }
}