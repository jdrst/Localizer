using System.Text.Json.Nodes;
using Localizer.Application.Abstractions;

namespace Localizer.Infrastructure.Configuration;

public class ConfigValueSetter(IPathProvider pathProvider) : IConfigValueSetter
{
    public async Task SetValueAsync(string key, string? value, bool isGlobal = true)
    {
        ArgumentNullException.ThrowIfNull(key, nameof(key));
        
        var path = isGlobal ? pathProvider.GlobalConfigFilePath : pathProvider.LocalConfigFilePath;
        
        var config = await JsonHelper.LoadAsync(path) ?? new JsonObject();

        var parts = key.Split(':');

        var node = config;
        if (parts.Length > 1)
        {
            foreach (var part in parts[..^1])
            {
                node[part] ??= new JsonObject();
                node = node[part]!;
            }
        }
        if (!string.IsNullOrWhiteSpace(value))
            node[parts.Last()] = value;
        else 
            node.AsObject().Remove(parts.Last());

        await config.WriteAsync(path);
    }
}