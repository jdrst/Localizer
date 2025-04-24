using System.Runtime.CompilerServices;
using DeepL;
using Localizer.Application.Abstractions;
using Localizer.Infrastructure.Files;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;

namespace Localizer.Tests.Infrastructure;

internal static class Mocks
{
    internal static ITranslator DeepLClientMock() => Substitute.For<ITranslator>();

    internal static ILogger<T> LoggerMock<T>() => Substitute.For<ILogger<T>>();

    internal static IOptions<T> OptionsMock<T>() where T : class => Substitute.For<IOptions<T>>();

    internal sealed class TestAppInfo : IAppInfo
    {
        public string Name() => nameof(TestAppInfo);
        public string Version() => "0.0.0";
    }
    internal sealed class TestPathProvider : IPathProvider, IDisposable
    {
        private readonly string _path;
        private readonly string _localConfigPath;
        private readonly string _globalConfigPath;

        private const string DefaultLocalConfig = """
                                                  {
                                                    "TranslationTextProviderType": "Prompt",
                                                    "DeepL": {
                                                      "AuthKey": "localKey",
                                                      "SourceLanguage": "en_us"
                                                    }
                                                  }
                                                  """;
        
        private const string DefaultGlobalConfig = """
                                                 {
                                                   "TranslationTextProviderType": "DeepL",
                                                   "DeepL": {
                                                     "AuthKey": "globalKey",
                                                     "Context": "some text",
                                                     "SourceLanguage": "fr"
                                                   }
                                                 }
                                                 """;
        public TestPathProvider([CallerMemberName] string methodName = "test")
        {
            _path = Path.Join(Path.GetTempPath(), methodName);
            Directory.CreateDirectory(_path);
            _localConfigPath = Path.Join(_path, PathProvider.LocalConfigFile);
            _globalConfigPath = Path.Join(_path, PathProvider.AppSettingsFile);
        }

        public TestPathProvider WithDefaults()
        {
            AddGlobalConfig(DefaultGlobalConfig);
            AddLocalConfig(DefaultLocalConfig);
            return this;
        }
        
        public void AddLocalConfig(string json) => File.WriteAllText(_localConfigPath, json);
        public void AddGlobalConfig(string json) => File.WriteAllText(_globalConfigPath, json);
        public string LocalConfigPath() => _localConfigPath;
        public string GlobalConfigPath() => _globalConfigPath;

        public void Dispose()
        {
            Directory.Delete(_path, true);
        }
    }
}