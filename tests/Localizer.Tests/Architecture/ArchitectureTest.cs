using Shouldly;

namespace Localizer.Tests.Architecture;

public class ArchitectureTest
{
    [Fact]
    public void TestInfrastructure()
    {
        var types = Types.In(ProjectNames.Infrastructure)
            .ShouldNot()
            .HaveDependencyOnAny(Types.NamesIn(ProjectNames.Executable));
        
        types.AssertSuccess(ProjectNames.Infrastructure);
    }
    
    [Fact]
    public void TestApplication()
    {
        var types = Types.In(ProjectNames.Application)
            .ShouldNot()
            .HaveDependencyOnAny(Types.NamesIn(ProjectNames.Application, ProjectNames.Executable));
        
        types.AssertSuccess(ProjectNames.Application);
    }
    
    [Fact]
    public void TestCore()
    {
        var types = Types.In(ProjectNames.Core)
            .ShouldNot()
            .HaveDependencyOtherThan("System", "Localizer.Core");
        
        types.AssertSuccess(ProjectNames.Core);
    }
}