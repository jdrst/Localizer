using System.Text.Json;
using System.Text.Json.Nodes;
using Localizer.Application.Abstractions;

namespace Localizer.Infrastructure.Configuration;

public class ConfigValueSetter(IPathProvider pathProvider) : IConfigValueSetter
{
    public async Task SetValueAsync(string key, string value, bool isGlobal = true)
    {
        ArgumentNullException.ThrowIfNull(key, nameof(key));
        
        var path = isGlobal ? pathProvider.GlobalConfigPath() : pathProvider.LocalConfigPath();
        
        var config = await JsonHelper.LoadAsync(path) ?? new JsonObject();

        var parts = key.Split(':');

        var node = config;
        if (parts.Length > 1)
        {
            foreach (var part in parts[..^1])
            {
                if (node[part] is null)
                    node[part] = new JsonObject();
                node = node[part]!;
            }
        }
        node[parts.Last()] = value;

        await JsonHelper.WriteAsync(config, path);
    }
}