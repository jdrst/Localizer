using System.Diagnostics.CodeAnalysis;
using Localizer.Infrastructure.Configuration;
using Localizer.Infrastructure.Provider.DeepL;

namespace Localizer.Tests;

public static class TestData
{
    internal static class Config
    {
        [SuppressMessage("ReSharper", "InconsistentNaming")]
        internal static class Keys
        {
            public const string TranslationProvider = nameof(AppOptions.TranslationProvider);
            public const string DeepL_SourceLanguage = $"{DeepLOptions.Section}:{nameof(DeepLOptions.SourceLanguage)}";
            public const string DeepL_Context = $"{DeepLOptions.Section}:{nameof(DeepLOptions.Context)}";
            public const string DeepL_AuthKey = $"{DeepLOptions.Section}:{nameof(DeepLOptions.AuthKey)}";
        }
        internal static class Global
        {
            public const string TranslationProvider = nameof(TranslationTextProviderType.DeepL);

            public static class DeepL
            {
                public const string AuthKey = "globalKey";
                public const string Context = "some text";
                public const string SourceLanguage = "fr";
            }
        }

        internal static class Local
        {
            public const string TranslationProvider = nameof(Infrastructure.Configuration.TranslationTextProviderType.ReplaceMe);

            public static class DeepL
            {
                public const string AuthKey = "localKey";
                public const string SourceLanguage = "en_us";
            }
        }
    }
    internal static class Json
    {
        internal const string DefaultLocalConfig = $$"""
                                                               {
                                                                 "{{nameof(AppOptions.TranslationProvider)}}": "{{Config.Local.TranslationProvider}}",
                                                                 "{{DeepLOptions.Section}}": {
                                                                   "{{nameof(DeepLOptions.AuthKey)}}": "{{Config.Local.DeepL.AuthKey}}",
                                                                   "{{nameof(DeepLOptions.SourceLanguage)}}": "{{Config.Local.DeepL.SourceLanguage}}"
                                                                 }
                                                               }
                                                               """;

        internal const string DefaultGlobalConfig = $$"""
                                                                {
                                                                  "{{nameof(AppOptions.TranslationProvider)}}": "{{Config.Global.TranslationProvider}}",
                                                                  "{{DeepLOptions.Section}}": {
                                                                    "{{nameof(DeepLOptions.AuthKey)}}": "{{Config.Global.DeepL.AuthKey}}",
                                                                    "{{nameof(DeepLOptions.Context)}}": "{{Config.Global.DeepL.Context}}",
                                                                    "{{nameof(DeepLOptions.SourceLanguage)}}": "{{Config.Global.DeepL.SourceLanguage}}"
                                                                  }
                                                                }
                                                                """;

        internal const string DefaultLocale = """
                                              {
                                                "TestKey": "Neutral",
                                                "OnlyHere": "Nür hiär",
                                                "Sub": {
                                                  "SubText": "Abc",
                                                  "SubText2": "Xyz",
                                                  "DoublyNested": {
                                                    "Something": "Something",
                                                    "Found": "found"
                                                  }
                                                }
                                              }
                                              """;

        internal const string DefaultLocaleEn = """
                                                {
                                                  "TestKey": "Neutral",
                                                  "Sub": {
                                                    "SubText": "Abc",
                                                    "DoublyNested": {
                                                        "Found": "found"
                                                    }
                                                  },
                                                  "NotInOther": "Foo",
                                                  "ObjectNotInOther": {
                                                    "SubValue": "value"
                                                  }
                                                }
                                                """;
    }
}