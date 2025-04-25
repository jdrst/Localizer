using System.Globalization;
using System.Text;
using Localizer.Application.Abstractions;
using Localizer.Core.Abstractions;
using Localizer.Infrastructure.Provider;
using Shouldly;

namespace Localizer.Tests.Architecture;

internal static class ProjectNames
{
    public const string Application = nameof(Application);
    public const string Infrastructure = nameof(Infrastructure);
    public const string Core = nameof(Core);
    public const string Executable = nameof(Executable);
}
internal static class Types
{
    internal static string[] NamesIn(params string[] projectNames)
    {
        var list = new List<string>();
        
        foreach (var projectName in projectNames)
            list.AddRange(In(projectName).GetTypes().Select(t => t.Name));

        return list.ToArray();
    }
    internal static NetArchTest.Rules.Types In(string projectName) => _typesMap[projectName];

    private static readonly Dictionary<string, NetArchTest.Rules.Types> _typesMap = new()
    {
        { ProjectNames.Application, NetArchTest.Rules.Types.InAssembly(typeof(IPathProvider).Assembly) },
        { ProjectNames.Core,NetArchTest.Rules.Types.InAssembly(typeof(ITranslationTextProvider).Assembly)},
        { ProjectNames.Infrastructure, NetArchTest.Rules.Types.InAssembly(typeof(ReplaceMeTranslationTextProvider).Assembly) },
        { ProjectNames.Executable, NetArchTest.Rules.Types.InAssembly(typeof(TypeRegistrar).Assembly) }
    };
}

internal static class Extensions
{
    internal static void AssertSuccess(this NetArchTest.Rules.ConditionList conditions, string failureMessage)
    {
        var result = conditions.GetResult();
        result.SelectedTypesForTesting.ShouldNotBeEmpty("No types were tested. Something went wrong.");
        result.IsSuccessful.ShouldBeTrue(result.GetFailureMessage(failureMessage));
    }

    private static string GetFailureMessage(this NetArchTest.Rules.TestResult result, string message) {
        var builder = new StringBuilder();
        builder.AppendLine(message);
        foreach (var t in result.FailingTypes)
        {
            builder.AppendLine(string.Create(CultureInfo.InvariantCulture, $"{t.FullName} {t.Explanation}"));
        }
        return builder.ToString();
    }
}