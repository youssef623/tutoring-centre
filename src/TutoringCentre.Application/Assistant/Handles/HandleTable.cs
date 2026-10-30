using System.Collections.ObjectModel;
using TutoringCentre.Application.Academics.Contracts;
using TutoringCentre.Domain.Common;
namespace TutoringCentre.Application.Assistant.Handles;
public sealed class HandleTable
{
    internal HandleTable(IDictionary<string, PortalChild> children) => Children = new ReadOnlyDictionary<string, PortalChild>(children);
    public IReadOnlyDictionary<string, PortalChild> Children { get; }
    public Result<Guid> Resolve(string? handle)
    {
        var key = handle?.Trim().ToLowerInvariant() ?? "";
        if (Guid.TryParse(key, out _)) return Result<Guid>.Failure(HandleError.RawIdRejected);
        return Children.TryGetValue(key, out var child) ? Result<Guid>.Success(child.Id) : Result<Guid>.Failure(HandleError.Unknown);
    }
}
