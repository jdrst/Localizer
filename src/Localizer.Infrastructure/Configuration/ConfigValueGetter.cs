using Localizer.Application.Abstractions;

namespace Localizer.Infrastructure.Configuration;

public class ConfigValueGetter(IPathProvider pathProvider) : IConfigValueGetter
{
    public async Task<(string?, string?)> GetValueAsync(string key)
    {
        ArgumentNullException.ThrowIfNull(key, nameof(key));
        
        var globalOptions = await JsonHelper.LoadAsync(pathProvider.GlobalConfigPath());
        var localOptions = await JsonHelper.LoadAsync(pathProvider.LocalConfigPath());

        var parts = key.Split(':');

        string? globalValue;
        string? localValue;
        if (parts.Length > 1)
            foreach (var part in parts[..^1])
            {
                globalOptions = globalOptions?[part];
                localOptions = localOptions?[part];
            }
        
        globalValue = globalOptions?[parts.Last()]?.GetValue<string>();
        localValue = localOptions?[parts.Last()]?.GetValue<string>();

        return (globalValue, localValue);
    }

    public async Task<(IDictionary<string, string>, IDictionary<string, string>)> ListValuesAsync()
    {
        var globalOptions = await JsonHelper.GetOptionsFrom(pathProvider.GlobalConfigPath()); 
        var localOptions = await JsonHelper.GetOptionsFrom(pathProvider.LocalConfigPath());
        return (globalOptions, localOptions);
    }
}