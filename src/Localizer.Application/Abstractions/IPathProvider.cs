namespace Localizer.Application.Abstractions;

public interface IPathProvider
{
    public string LocalConfigFilePath { get; }
    public string GlobalConfigFilePath { get; }
}