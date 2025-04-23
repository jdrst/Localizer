namespace Localizer.Application.Abstractions;

public interface IPathProvider
{
    public string LocalConfigPath();
    public string GlobalConfigPath();
}