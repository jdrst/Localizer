namespace Localizer.Application.Abstractions;

public interface IPathProvider
{
    public string Root { get; }
    public string LocalConfigPath { get; }
    public string GlobalConfigPath { get; }
    public string RelativeLocalConfigPath { get; }
}