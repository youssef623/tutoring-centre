namespace TutoringCentre.Domain.Identity;

/// <summary>Membership role discriminator, retaining its original public name for session compatibility.
/// Staff entry points accept only Owner, Secretary and Teacher; portal creation uses PortalRole.</summary>
public enum StaffRole
{
    Owner,
    Teacher,
    Secretary,
    Parent,
    Student,
}
