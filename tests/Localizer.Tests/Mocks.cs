using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using DeepL;
using Localizer.Application.Abstractions;
using Localizer.Infrastructure.Files;
using Microsoft.Extensions.Options;
using NSubstitute;

namespace Localizer.Tests;

[SuppressMessage("Maintainability", "CA1515:Consider making public types internal")]
public static class Mocks
{
    internal static ITranslator TestDeepLClient() => Substitute.For<ITranslator>();
    
    internal static IOptions<T> TestOptions<T>() where T : class => Substitute.For<IOptions<T>>();
    
    [SuppressMessage("Design", "CA1034:Nested types should not be visible")]
    public sealed class TestPathProvider : IPathProvider, IDisposable
    {
        private readonly string _path;
        private readonly string _localConfigPath;
        private readonly string _globalConfigPath;
        
        public string Root => _path;
        public string LocalConfigFilePath => _localConfigPath;
        public string GlobalConfigFilePath => _globalConfigPath;
        public TestPathProvider([CallerMemberName] string methodName = "test")
        {
            var random = Path.GetFileNameWithoutExtension(Path.GetRandomFileName());
            _path = Path.Join(Path.GetTempPath(), $"{random}{methodName}");
            Directory.CreateDirectory(_path);
            _localConfigPath = Path.Join(_path, PathProvider.LocalConfigFile);
            _globalConfigPath = Path.Join(_path, PathProvider.AppSettingsFile);
        }

        public TestPathProvider WithDefaultConfig()
        {
            AddGlobalConfig(TestData.Json.DefaultGlobalConfig);
            AddLocalConfig(TestData.Json.DefaultLocalConfig);
            return this;
        }
        public void AddLocalConfig(string json) => File.WriteAllText(_localConfigPath, json);
        public void AddGlobalConfig(string json) => File.WriteAllText(_globalConfigPath, json);

        public void Dispose()
        {
            if (Directory.Exists(_path))
                Directory.Delete(_path, true);
        }
    }
}