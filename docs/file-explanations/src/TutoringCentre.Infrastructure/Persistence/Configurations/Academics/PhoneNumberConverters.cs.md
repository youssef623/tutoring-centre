# `src/TutoringCentre.Infrastructure/Persistence/Configurations/Academics/PhoneNumberConverters.cs`

## Purpose

Source summary (intent, not independent execution proof): The persistence half of PhoneNumber (Task 38.1): stored as its E.164 string in one column, for both a required and an optional phone. Reading a column value that no longer passes PhoneNumber.Create means the row holds corrupt data; that failure surfaces as an exception here, not a Result, since a value converter has no way to report one through EF's materialisation path.

## Where it fits

Category: **source**. [Open current file](../../../../../../../src/TutoringCentre.Infrastructure/Persistence/Configurations/Academics/PhoneNumberConverters.cs). [Complete file index](../../../../../INDEX.md). [Current teaching overview](../../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 21 lines and 1,013 bytes. Source hash: `b7f03cfeed4ec8a26e809522716720a6ca792b286f60ae9128193b22776a4f6c`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using Microsoft.EntityFrameworkCore.Storage.ValueConversion;`
- Line 2: `using TutoringCentre.Domain.Common;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 12](../../../../../../../src/TutoringCentre.Infrastructure/Persistence/Configurations/Academics/PhoneNumberConverters.cs#L12): `internal static class PhoneNumberConverters`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [src/TutoringCentre.Infrastructure/Persistence/Configurations/Academics/ParentConfiguration.cs:32](../../../../../../../src/TutoringCentre.Infrastructure/Persistence/Configurations/Academics/ParentConfiguration.cs#L32)
- [src/TutoringCentre.Infrastructure/Persistence/Configurations/Academics/StudentConfiguration.cs:42](../../../../../../../src/TutoringCentre.Infrastructure/Persistence/Configurations/Academics/StudentConfiguration.cs#L42)
- [src/TutoringCentre.Infrastructure/Persistence/Configurations/Academics/TeacherConfiguration.cs:42](../../../../../../../src/TutoringCentre.Infrastructure/Persistence/Configurations/Academics/TeacherConfiguration.cs#L42)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
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
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
