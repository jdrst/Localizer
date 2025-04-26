using DeepL;
using DeepL.Model;
using Localizer.Application.Commands;
using Localizer.Infrastructure.Configuration;
using Localizer.Infrastructure.Provider.DeepL;
using NSubstitute;
using Shouldly;
using Spectre.Console.Testing;

namespace Localizer.Tests.IntegrationTests.Commands;

public class TranslateCommandTest : IntegrationTest
{
    [Fact]
    public async Task TestReplaceMe()
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
    public async Task TestPrompt()
    {
        var app = DefaultCommandAppTester();
        TestConsole.Interactive();
        TestConsole.Input.PushTextWithEnter("foo");
        TestConsole.Input.PushTextWithEnter("bar");
        TestConsole.Input.PushTextWithEnter("baz");

        TestPathProvider!.AddLocalConfig($$"""{"{{nameof(AppOptions.TranslationProvider)}}":"{{nameof(TranslationTextProviderType.Prompt)}}"}""");
        
        var paths = LocaleFileProvider.DefaultLocales();

        var result = await app.RunAsync(TranslateCommand.Name, Path.GetFileName(paths.First()));
        
        result.ExitCode.ShouldBe(0);
        var locale = await File.ReadAllTextAsync(paths.First(), TestContext.Current.CancellationToken);
        var localeEn = await File.ReadAllTextAsync(paths.Last(), TestContext.Current.CancellationToken);
        await Verify((result.Output, locale, localeEn));
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

        var responses = new TextResult[3];
        Array.Fill(responses, new TextResult("foo bar", "de", 12, "modelType"));
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
                Arg.Any<string[]>(), 
                Arg.Any<string>(), 
                Arg.Any<string?>()!,
                Arg.Any<TextTranslateOptions>(), 
                Arg.Any<CancellationToken>())
            .Returns(responses);
        
        appBuilder.ReplaceService<ITranslator>(deepLClient);
        var app = appBuilder.Build();

        
        var paths = LocaleFileProvider.DefaultLocales();
        var localeUzPath =LocaleFileProvider.AddLocales(new LocaleHelper("{}", "uz-Latn-UZ"))[0];

        var result = await app.RunAsync(TranslateCommand.Name, Path.GetFileName(paths.First()));
        
        result.ExitCode.ShouldBe(0);
        result.Output.ShouldContain("Translating to");
        result.Output.ShouldContain("Characters billed this session: ");
        result.Output.ShouldContain("[Info]: DeepL can't translate to EN, using EN-US instead");
        result.Output.ShouldContain("[Info]: Characters billed this session:");
        var locale = await File.ReadAllTextAsync(paths.First(), TestContext.Current.CancellationToken);
        var localeEn = await File.ReadAllTextAsync(paths.Last(), TestContext.Current.CancellationToken);
        var localeUz = await File.ReadAllTextAsync(localeUzPath, TestContext.Current.CancellationToken);
        await Verify((locale, localeEn, localeUz));
    }
}