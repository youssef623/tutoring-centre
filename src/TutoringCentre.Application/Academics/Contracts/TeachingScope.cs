namespace TutoringCentre.Application.Academics.Contracts;

public sealed record TeachingScopeKind
{
    public static readonly TeachingScopeKind All = new("all");
    public static readonly TeachingScopeKind Own = new("own");
    public static readonly TeachingScopeKind None = new("none");

    private TeachingScopeKind(string name) => Name = name;
    public string Name { get; }
}

public sealed record TeachingScope
{
    public static readonly TeachingScope All = new(TeachingScopeKind.All, null);
    public static readonly TeachingScope None = new(TeachingScopeKind.None, null);

    private TeachingScope(TeachingScopeKind kind, Guid? teacherId)
    {
        Kind = kind;
        TeacherId = teacherId;
    }

    public TeachingScopeKind Kind { get; }
    public Guid? TeacherId { get; }

    public static TeachingScope Own(Guid teacherId) =>
        teacherId == Guid.Empty ? None : new(TeachingScopeKind.Own, teacherId);

    public bool Allows(Guid teacherId) =>
        Kind == TeachingScopeKind.All || (Kind == TeachingScopeKind.Own && TeacherId == teacherId);
}
