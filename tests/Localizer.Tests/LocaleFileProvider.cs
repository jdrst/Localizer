namespace Localizer.Tests;

public record LocaleHelper(string Json, string? Postfix = null);

public sealed class LocaleFileProvider : IDisposable
{
    private readonly string _basePath = Environment.CurrentDirectory;
    private string[] _paths = [];

    public string[] DefaultLocales() => AddLocales(new LocaleHelper(TestData.Json.DefaultLocale), new LocaleHelper(TestData.Json.DefaultLocaleEn, "en"));

    public string[] AddLocales(params LocaleHelper[] jsons)
    {
        ArgumentNullException.ThrowIfNull(jsons);
        
        _paths = new string[jsons.Length];
        foreach (var (idx,(json, postfix)) in jsons.Index())
        {
            var fileName = "locale";
            if (postfix is not null)
                fileName += $"_{postfix}";
            fileName += ".json";
            var path = Path.Join(_basePath, fileName);
            File.WriteAllText(path, json);
            _paths[idx] = path;
        }
        return _paths;
    }

    public void Dispose()
    {
        foreach (var file in _paths)
            File.Delete(file);
    }
}