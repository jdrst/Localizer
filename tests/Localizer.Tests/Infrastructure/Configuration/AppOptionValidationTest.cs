using Localizer.Infrastructure.Configuration;
using Localizer.Infrastructure.Provider.DeepL;
using Shouldly;

namespace Localizer.Tests.Infrastructure.Configuration;

public class AppOptionValidationTest
{
    [Theory]
    [InlineData(TranslationTextProviderType.DeepL, "abc")]
    [InlineData(TranslationTextProviderType.Prompt, "")]
    public void TestSuccess(TranslationTextProviderType type, string authKey)
    {
        var options = new AppOptions
        {
            TranslationTextProviderType = type,
            DeepL = new DeepLOptions {AuthKey = authKey}
        };

        var result = new AppOptionValidation().Validate(null, options);
        result.Succeeded.ShouldBeTrue();
    }
    
    [Fact]
    public void TestFailureNoOptions()
    {
        var result = new AppOptionValidation().Validate(null, null);
        result.Succeeded.ShouldBeFalse();
        result.Failures.ShouldHaveSingleItem();
        result.Failures.First().ShouldBe("No configuration found.");
    }
    [Fact]
    public void TestFailureDeepLAuthKey()
    {
        var options = new AppOptions
        {
            TranslationTextProviderType = TranslationTextProviderType.DeepL,
        };

        var result = new AppOptionValidation().Validate(null, options);
        result.Succeeded.ShouldBeFalse();
        result.Failures.ShouldHaveSingleItem();
        result.Failures.First().ShouldBe("No DeepL AuthKey provided.");
    }
}