using TutoringCentre.Application.Common.Security;

namespace TutoringCentre.Application.Tests.Security;

public sealed class TeacherScopeTests
{
    [Fact]
    public void All_AllowsAnyTeacher()
    {
        Assert.True(TeacherScope.All.Allows(Guid.CreateVersion7()));
    }

    [Fact]
    public void None_AllowsNoTeacher()
    {
        Assert.False(TeacherScope.None.Allows(Guid.CreateVersion7()));
    }

    [Fact]
    public void Own_AllowsExactlyItsTeacher()
    {
        var teacherId = Guid.CreateVersion7();
        var scope = TeacherScope.Own(teacherId);

        Assert.True(scope.Allows(teacherId));
        Assert.False(scope.Allows(Guid.CreateVersion7()));
    }
}
