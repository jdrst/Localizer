using System.Globalization;
using DeepL;
using DeepL.Model;
using Localizer.Infrastructure.Logging;
using Localizer.Infrastructure.Provider.DeepL;
using NSubstitute;
using Shouldly;

namespace Localizer.Tests.UnitTests.Infrastructure.Provider;

public class DeepLTranslationTextProviderTest
{
     [Fact]
     public async Task TestGetTranslationFor()
     {
         var deeplClient = Mocks.TestDeepLClient();
         deeplClient.TranslateTextAsync(
                 Arg.Any<string>(), 
                 Arg.Any<string>(), 
                 Arg.Any<string?>()!,
                 Arg.Any<TextTranslateOptions>(), 
                 TestContext.Current.CancellationToken)
             .Returns(new TextResult("foo bar", "de", 12, "modelType"));
         var logger = Mocks.TestLogger<DeepLTranslationTextProvider>();
         var options = Mocks.TestOptions<DeepLOptions>();
         using var provider = new DeepLTranslationTextProvider(logger, options,deeplClient, new Mocks.TestAppInfo());
     
         var input = "bar foo";
         var result = await provider.GetTranslationFor(input, new CultureInfo("en_US"), TestContext.Current.CancellationToken);
         
         provider.UsesConsole().ShouldBeFalse();
         logger.Received(1).DebugCharactersUsed(12);
         result.ShouldBe($"foo bar");
     }
}