using System.Globalization;
using TutoringCentre.Application.Academics.Contracts;
namespace TutoringCentre.Application.Assistant.Handles;
public static class HandleResolver
{
    public static HandleTable Mint(IReadOnlyList<PortalChild> children, bool isStudent = false)
    {
        ArgumentNullException.ThrowIfNull(children);
        if (isStudent && children.Count > 1) throw new ArgumentException("A student has only one subject.");
        var ordered = children.OrderBy(x => x.Name, StringComparer.Ordinal).ThenBy(x => x.Id).ToArray();
        return new HandleTable(ordered.Select((child, i) => new KeyValuePair<string, PortalChild>(isStudent ? "me" : "child_" + (i + 1).ToString(CultureInfo.InvariantCulture), child))
            .ToDictionary(x => x.Key, x => x.Value, StringComparer.Ordinal));
    }
}
