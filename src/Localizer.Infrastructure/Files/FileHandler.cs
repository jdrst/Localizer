using System.Globalization;
using System.Text.Json.Nodes;
using Localizer.Application;
using Localizer.Application.Abstractions;

namespace Localizer.Infrastructure.Files;

public class FileHandler : IFileHandler
{
    private JsonObject _base = null!;
    private CulturedJson[] _culturedJsons = [];
    private readonly List<Message> _messages = [];
    
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
        
        _base = (await JsonHelper.LoadAsync(baseFilePath))!.AsObject();
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

            var node = (await JsonHelper.LoadAsync(filePath))!.AsObject();
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
    private static async Task WriteFileAsync(string path, string? prefix, JsonObject node)
    {
        var outPath = string.IsNullOrWhiteSpace(prefix)
            ? path
            : $"{prefix}_{Path.GetFileNameWithoutExtension(path)}.json";
        
        await node.WriteAsync(outPath);
    }
}