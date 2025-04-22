using System.Text.Json.Nodes;

namespace Localizer.Abstractions;

public interface IFileHandler
{
    public IReadOnlyList<Error> Errors();
    public JsonObject Base();
    public Task<bool> InitializeAsync(string baseFilePath);

    public CulturedJson[] CulturedJsons();

    public Task WriteFilesAsync(string? prefix);
}