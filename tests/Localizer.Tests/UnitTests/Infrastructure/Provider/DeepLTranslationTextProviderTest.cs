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
     public async Task TestGetTranslationsAsync()
     {
         var deepLClient = Mocks.TestDeepLClient();
         deepLClient.TranslateTextAsync(
                 Arg.Any<string[]>(), 
                 Arg.Any<string>(), 
                 Arg.Any<string?>()!,
                 Arg.Any<TextTranslateOptions>(), 
                 TestContext.Current.CancellationToken)
             .Returns([new TextResult("foo bar", "de", 12, "modelType")]);
         var options = Mocks.TestOptions<DeepLOptions>();
         var provider = new DeepLTranslationTextProvider(options, deepLClient, new AppInfo());
     
         var input = "bar foo";
         var result = await provider.GetTranslationsAsync([input], new CultureInfo("en-US"), TestContext.Current.CancellationToken);
         
         provider.UsesConsole.ShouldBeFalse();
         provider.Messages.ShouldHaveSingleItem();
         provider.Messages[0].MessageType.ShouldBe(MessageType.Info);
         provider.Messages[0].Text.ShouldEndWith("12");
         result.ShouldHaveSingleItem();
         result[0].ShouldBe($"foo bar");
     }
}