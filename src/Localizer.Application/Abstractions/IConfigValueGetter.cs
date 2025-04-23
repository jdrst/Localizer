namespace Localizer.Application.Abstractions;

public interface IConfigValueGetter
{
    public Task<(string? globalValue, string? localValue)> GetValueAsync(string key);
    
    public Task<(IDictionary<string, string> globalValues, IDictionary<string, string> localValues)> ListValuesAsync();
}