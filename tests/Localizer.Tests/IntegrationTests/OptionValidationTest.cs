using Localizer.Infrastructure.Configuration;
using Localizer.Infrastructure.Provider.DeepL;
using Shouldly;
using Spectre.Console.Cli;

namespace Localizer.Tests.IntegrationTests;

public class OptionValidationTest : IntegrationTest
{
    [Fact]
    public async Task TestInvalidConfig()
    {
        var appBuilder = DefaultCommandAppBuilder();
        TestPathProvider!.AddGlobalConfig($$"""{"{{nameof(AppOptions.TranslationProvider)}}":"{{nameof(TranslationTextProviderType.DeepL)}}"}""");
        var app = appBuilder.Build();

        var ex = await app.RunAsync("translate", "doesntexistbutwhocares.json").ShouldThrowAsync<CommandRuntimeException>();
        ex.InnerException?.Message.ShouldBe(DeepLOptionValidation.NoAuthKeyProvidedMessage);
    }
}