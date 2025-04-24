using Localizer.Infrastructure.Configuration;
using Localizer.Infrastructure.Provider.DeepL;
using Microsoft.Extensions.Configuration;
using Shouldly;
using ConfigurationProvider = Localizer.Infrastructure.Configuration.ConfigurationProvider;

namespace Localizer.Tests.UnitTests.Infrastructure.Configuration;

public class ConfigurationProviderTest
{
    [Fact]
    public void TestLocalOverGlobal()
    {
        using var pathProvider = new Mocks.TestPathProvider().WithDefaultConfig();

        var configurationProvider = new ConfigurationProvider(pathProvider);

        var appOptions = new AppOptions();
        configurationProvider.GetConfig().Bind(appOptions);
        var deeplOptions = new DeepLOptions();
        configurationProvider.GetSection(DeepLOptions.Section).Bind(deeplOptions);
        appOptions.TranslationProvider.ToString().ShouldBe(TestData.Config.Local.TranslationProvider); //local over global
        deeplOptions.Context.ShouldBe(TestData.Config.Global.DeepL.Context); //no local -> global
    }
}