using DeepL;
using DeepL.Model;
using Localizer.Application.Commands;
using Localizer.Infrastructure.Configuration;
using Localizer.Infrastructure.Provider.DeepL;
using NSubstitute;
using Shouldly;
using Spectre.Console.Cli;

namespace Localizer.Tests.IntegrationTests.Commands;

public class TranslateCommandTest : IntegrationTest
{
    [Fact]
    public async Task TestSimple()
    {
        var app = DefaultCommandAppTester();
        
        var paths = LocaleFileProvider.DefaultLocales();

        var result = await app.RunAsync(TranslateCommand.Name, Path.GetFileName(paths.First()));
        
        result.ExitCode.ShouldBe(0);
        result.Output.ShouldContain("Translating to English.");
        var locale = await File.ReadAllTextAsync(paths.First(), TestContext.Current.CancellationToken);
        var localeEn = await File.ReadAllTextAsync(paths.Last(), TestContext.Current.CancellationToken);
        await Verify((locale, localeEn));
    }
    
    [Fact]
    public async Task TestFileDoesNotExist()
    {
        var app = DefaultCommandAppTester();
        
        var result = await app.RunAsync(TranslateCommand.Name, "does_not.exist");
        
        result.ExitCode.ShouldBe(-1);
        result.Output.ShouldStartWith($"File 'does_not.exist' doesn't exists in");
    }
    
    [Fact]
    public async Task TestDeepL()
    {
        var appBuilder = DefaultCommandAppBuilder();
        TestPathProvider!.AddGlobalConfig($$"""
                                            {
                                            "{{nameof(AppOptions.TranslationProvider)}}":"{{nameof(TranslationTextProviderType.DeepL)}}",
                                            "{{DeepLOptions.Section}}":{
                                                "{{nameof(DeepLOptions.AuthKey)}}":"1234"
                                                }
                                            }
                                            """);
        var deepLClient = Mocks.TestDeepLClient();
        deepLClient.TranslateTextAsync(
                Arg.Any<string>(), 
                Arg.Any<string>(), 
                Arg.Any<string?>()!,
                Arg.Any<TextTranslateOptions>(), 
                Arg.Any<CancellationToken>())
            .Returns(new TextResult("foo bar", "de", 12, "modelType"));
        
        appBuilder.ReplaceService<ITranslator>(deepLClient);
        var app = appBuilder.Build();

        
        var paths = LocaleFileProvider.DefaultLocales();

        var result = await app.RunAsync(TranslateCommand.Name, Path.GetFileName(paths.First()));
        
        result.ExitCode.ShouldBe(0);
        var locale = await File.ReadAllTextAsync(paths.First(), TestContext.Current.CancellationToken);
        var localeEn = await File.ReadAllTextAsync(paths.Last(), TestContext.Current.CancellationToken);
        await Verify((result.Output, locale, localeEn));
    }
}