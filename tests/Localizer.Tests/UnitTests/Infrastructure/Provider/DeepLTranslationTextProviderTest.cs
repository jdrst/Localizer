using System.Globalization;
using DeepL;
using DeepL.Model;
using Localizer.Core;
using Localizer.Infrastructure.Provider.DeepL;
using NSubstitute;
using Shouldly;

namespace Localizer.Tests.UnitTests.Infrastructure.Provider;

public class DeepLTranslationTextProviderTest
{
     [Fact]
     public async Task TestGetTranslationFor()
     {
         var deepLClient = Mocks.TestDeepLClient();
         deepLClient.TranslateTextAsync(
                 Arg.Any<string>(), 
                 Arg.Any<string>(), 
                 Arg.Any<string?>()!,
                 Arg.Any<TextTranslateOptions>(), 
                 TestContext.Current.CancellationToken)
             .Returns(new TextResult("foo bar", "de", 12, "modelType"));
         var options = Mocks.TestOptions<DeepLOptions>();
         using var provider = new DeepLTranslationTextProvider(options, deepLClient, new Mocks.TestAppInfo());
     
         var input = "bar foo";
         var result = await provider.GetTranslationFor(input, new CultureInfo("en_US"), TestContext.Current.CancellationToken);
         
         provider.UsesConsole.ShouldBeFalse();
         provider.Messages.ShouldHaveSingleItem();
         provider.Messages[0].MessageType.ShouldBe(MessageType.Info);
         provider.Messages[0].Text.ShouldEndWith("12");
         result.ShouldBe($"foo bar");
     }
}