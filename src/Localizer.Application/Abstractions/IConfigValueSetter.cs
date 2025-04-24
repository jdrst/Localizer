namespace Localizer.Application.Abstractions;

public interface IConfigValueSetter
{
    public Task SetValueAsync(string key, string? value, bool isGlobal = true);
}