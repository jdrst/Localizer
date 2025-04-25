using Localizer.Infrastructure.Configuration;
using Shouldly;

namespace Localizer.Tests.UnitTests.Infrastructure.Configuration;

public class ConfigValueGetterTest
{
    [Theory]
    [InlineData(TestData.Config.Keys.TranslationProvider, "DeepL","ReplaceMe")]
    [InlineData(TestData.Config.Keys.DeepL_SourceLanguage, "fr","en_us")]
    [InlineData(TestData.Config.Keys.DeepL_Context, "some text",null)]
    [InlineData("unknown", null,null)]
    public async Task TestGet(string key, string? expectedGlobalValue, string? expectedLocalValue)
    {
        using var pathProvider = new Mocks.TestPathProvider().WithDefaultConfig();
        var valueGetter = new ConfigValueGetter(pathProvider);

        var (global, local) = await valueGetter.GetValueAsync(key);
        
        global.ShouldBe(expectedGlobalValue);
        local.ShouldBe(expectedLocalValue);
    }
    
    [Fact]
    public async Task TestList()
    {
        using var pathProvider = new Mocks.TestPathProvider().WithDefaultConfig();
        var valueGetter = new ConfigValueGetter(pathProvider);
        
        await Verify(await valueGetter.ListValuesAsync());
    }
}