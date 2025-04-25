using Localizer.Application.Abstractions;

namespace Localizer.Infrastructure.Configuration;

public class ConfigValueGetter(IPathProvider pathProvider) : IConfigValueGetter
{
    public async Task<(string?, string?)> GetValueAsync(string key)
    {
        ArgumentNullException.ThrowIfNull(key, nameof(key));
        
        var globalOptions = await JsonHelper.LoadAsync(pathProvider.GlobalConfigFilePath);
        var localOptions = await JsonHelper.LoadAsync(pathProvider.LocalConfigFilePath);

        var parts = key.Split(':');

        if (parts.Length > 1)
            foreach (var part in parts[..^1])
            {
                globalOptions = globalOptions?[part];
                localOptions = localOptions?[part];
            }
        
        var globalValue = globalOptions?[parts.Last()]?.GetValue<string>();
        var localValue = localOptions?[parts.Last()]?.GetValue<string>();

        return (globalValue, localValue);
    }

    public async Task<(IDictionary<string, string> globalValues, IDictionary<string, string> localValues)> ListValuesAsync()
    {
        var globalOptions = await JsonHelper.GetOptionsAsync(pathProvider.GlobalConfigFilePath); 
        var localOptions = await JsonHelper.GetOptionsAsync(pathProvider.LocalConfigFilePath);
        return (globalOptions, localOptions);
    }
}