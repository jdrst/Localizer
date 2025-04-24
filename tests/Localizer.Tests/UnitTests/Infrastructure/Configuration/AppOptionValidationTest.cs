using Localizer.Infrastructure.Configuration;
using Localizer.Infrastructure.Provider.DeepL;
using Shouldly;

namespace Localizer.Tests.UnitTests.Infrastructure.Configuration;

public class AppOptionValidationTest
{
    [Fact]
    public void TestFailureNoOptions()
    {
        var result = new AppOptionValidation().Validate(null, null);
        result.Succeeded.ShouldBeFalse();
        result.Failures.ShouldHaveSingleItem();
        result.Failures.First().ShouldBe(AppOptionValidation.NoOptionsFoundText);
    }
    
    [Fact]
    public void TestFailureDeepLAuthKey()
    {
        var options = new DeepLOptions();

        var result = new DeepLOptionValidation().Validate(null, options);
        
        result.Succeeded.ShouldBeFalse();
        result.Failures.ShouldHaveSingleItem();
        result.Failures.First().ShouldBe(DeepLOptionValidation.NoAuthKeyProvidedMessage);
    }
}