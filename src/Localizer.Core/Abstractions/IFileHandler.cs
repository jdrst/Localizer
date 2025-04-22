using System.Text.Json.Nodes;

namespace Localizer.Core.Abstractions;

public interface IFileHandler
{
    public IReadOnlyList<Message> Messages();
    public JsonObject Base();
    public Task<bool> InitializeAsync(string baseFilePath);

    public CulturedJson[] CulturedJsons();

    public Task WriteFilesAsync(string? prefix);
}