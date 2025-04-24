using Localizer.Infrastructure.Configuration;
using Localizer.Infrastructure.Provider.DeepL;

namespace Localizer.Tests.UnitTests.Infrastructure.Configuration;

public class ConfigValueSetterTest
{
    [Theory]
    [InlineData(TestData.Config.Keys.TranslationProvider, "Prompt", false)]     //set local
    [InlineData(TestData.Config.Keys.DeepL_SourceLanguage, "es-ES", true)]      //set global nested
    [InlineData(TestData.Config.Keys.DeepL_Context, "some local text",false)]   //set local nested
    [InlineData("unknown", "weird that i can do this",true)]                    //set anything (?) TODO
    [InlineData(TestData.Config.Keys.DeepL_SourceLanguage, null,true)]          //unset global with null
    [InlineData(TestData.Config.Keys.DeepL_SourceLanguage, "",true)]            //unset with emtpy string
    [InlineData(TestData.Config.Keys.TranslationProvider, null, false)]         //unset local
    [InlineData(DeepLOptions.Section, null, true)]                              //unset section
    [InlineData(DeepLOptions.Section, "", true)]                                //unset section with emtpy string
    public async Task TestSet(string key, string? value, bool isGlobal)
    {
        using var pathProvider = new Mocks.TestPathProvider().WithDefaultConfig();
        var valueSetter = new ConfigValueSetter(pathProvider);

        await valueSetter.SetValueAsync(key, value, isGlobal);
        
        await Verify(File.ReadAllTextAsync(isGlobal ? pathProvider.GlobalConfigPath : pathProvider.LocalConfigPath, TestContext.Current.CancellationToken));
    }
}