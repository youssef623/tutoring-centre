using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Infrastructure.Persistence.Configurations.Academics;

/// <summary>
/// The persistence half of PhoneNumber (Task 38.1): stored as its E.164 string in one column, for both a
/// required and an optional phone. Reading a column value that no longer passes PhoneNumber.Create means the
/// row holds corrupt data; that failure surfaces as an exception here, not a Result, since a value converter
/// has no way to report one through EF's materialisation path.
/// </summary>
internal static class PhoneNumberConverters
{
    public static readonly ValueConverter<PhoneNumber, string> Required = new(
        phone => phone.Value,
        value => PhoneNumber.Create(value).Value);

    public static readonly ValueConverter<PhoneNumber?, string?> Optional = new(
        phone => phone == null ? null : phone.Value,
        value => value == null ? null : PhoneNumber.Create(value).Value);
}
