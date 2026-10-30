namespace TutoringCentre.Application.Common.Cqrs;
/// <summary>Server-configured command budgets, including begin, save and commit.</summary>
public interface ICommandTimeoutPolicy
{
    TimeSpan? GetTimeout(object command);
}
