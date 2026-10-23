namespace TutoringCentre.Application.Common.Security;

/// <summary>The three shapes a group-returning read can be limited to.</summary>
public enum TeacherScopeKind
{
    All,
    None,
    Own,
}

/// <summary>
/// The ownership guard's result for one request: every group, no group, or only the groups of one teacher
/// record. <see cref="None"/> is distinct from <see cref="All"/> on purpose — a Teacher-role actor with no
/// linked record must see nothing, never everything.
/// </summary>
public sealed record TeacherScope
{
    public static readonly TeacherScope All = new(TeacherScopeKind.All, null);
    public static readonly TeacherScope None = new(TeacherScopeKind.None, null);

    private TeacherScope(TeacherScopeKind kind, Guid? teacherId)
    {
        Kind = kind;
        TeacherId = teacherId;
    }

    public TeacherScopeKind Kind { get; }

    /// <summary>Set only when <see cref="Kind"/> is <see cref="TeacherScopeKind.Own"/>.</summary>
    public Guid? TeacherId { get; }

    public static TeacherScope Own(Guid teacherId) => new(TeacherScopeKind.Own, teacherId);

    /// <summary>Whether a group owned by <paramref name="teacherId"/> is within this scope.</summary>
    public bool Allows(Guid teacherId) => Kind switch
    {
        TeacherScopeKind.All => true,
        TeacherScopeKind.None => false,
        TeacherScopeKind.Own => TeacherId == teacherId,
        _ => false,
    };
}
