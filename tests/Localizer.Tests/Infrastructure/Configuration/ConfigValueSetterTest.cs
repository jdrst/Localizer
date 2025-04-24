using Localizer.Infrastructure.Configuration;
using Shouldly;

namespace Localizer.Tests.Infrastructure.Configuration;

public class ConfigValueSetterTest
{
    [Theory]
    [InlineData("TranslationTextProviderType", "ReplaceMe", false)]
    [InlineData("DeepL:SourceLanguage", "es-ES", true)]
    [InlineData("DeepL:Context", "some local text",false)]
    [InlineData("unknown", "weird that i can do this",true)]
    [InlineData("DeepL:SourceLanguage", null,true)]
    [InlineData("TranslationTextProviderType", null, false)]
    [InlineData("DeepL", null, true)]
    public async Task TestSet(string key, string? value, bool isGlobal)
    {
        using var pathProvider = new Mocks.TestPathProvider().WithDefaults();
        var valueSetter = new ConfigValueSetter(pathProvider);

        await valueSetter.SetValueAsync(key, value, isGlobal);
        
        await Verify(File.ReadAllTextAsync(isGlobal ? pathProvider.GlobalConfigPath() : pathProvider.LocalConfigPath(), TestContext.Current.CancellationToken));
    }
}