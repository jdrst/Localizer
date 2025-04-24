using Localizer.Infrastructure.Configuration;
using Shouldly;

namespace Localizer.Tests.Infrastructure.Configuration;

public class ConfigValueGetterTest
{
    [Theory]
    [InlineData("TranslationTextProviderType", "DeepL","Prompt")]
    [InlineData("DeepL:SourceLanguage", "fr","en_us")]
    [InlineData("DeepL:Context", "some text",null)]
    [InlineData("unknown", null,null)]
    public async Task TestGet(string key, string? expectedGlobalValue, string? expectedLocalValue)
    {
        using var pathProvider = new Mocks.TestPathProvider().WithDefaults();
        var valueGetter = new ConfigValueGetter(pathProvider);

        var (global, local) = await valueGetter.GetValueAsync(key);
        
        global.ShouldBe(expectedGlobalValue);
        local.ShouldBe(expectedLocalValue);
    }
    
    [Fact]
    public Task TestList()
    {
        using var pathProvider = new Mocks.TestPathProvider().WithDefaults();
        var valueGetter = new ConfigValueGetter(pathProvider);
        
        return Verify(valueGetter.ListValuesAsync());
    }
}