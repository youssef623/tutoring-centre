# `src/TutoringCentre.Domain`

Part of the [file index](INDEX.md). Domain depends on **nothing**: its `.csproj` has no `ProjectReference` and no `PackageReference` (only `InternalsVisibleTo` for `TutoringCentre.Architecture.Tests`). Concepts: overview §6.4 (Result), §6.6 (entities).

## `Common/Result.cs`
**Purpose:** return value for operations that can fail for *expected* reasons.

- `class Result` — protected constructor `(bool isSuccess, Error? error)` enforces the invariant: success⇒no error (`ArgumentException`), failure⇒error present (`ArgumentNullException`). Properties `IsSuccess`, `IsFailure` (`!IsSuccess`), `Error`. Static `Success()` and `Failure(Error)`.
- `sealed class Result<T> : Result` — private constructors (so only the factories construct it). `Value` returns the value on success and **throws `InvalidOperationException`** on failure ("reading it is a bug"). `Success(T)` / `Failure(Error)` — `Failure` is declared `static new` because it hides the base `Result.Failure` with the generic return type.
- `[SuppressMessage CA1000]`: static members on generic types are normally discouraged; justified as "the agreed factory API".
- On failure `_value = default!` — the null-forgiving operator tells the compiler "I know this is null/default; `Value` guards access".

**Used by:** every handler, `Centre.Create`, `ResultHttpExtensions`, the dispatcher. **Tests:** `ResultTests` (4).

## `Common/Error.cs` and `ErrorKind.cs`
`sealed record Error(string Code, string Message, ErrorKind Kind, IReadOnlyDictionary<string,string[]>? Fields = null)`. A **record** gives value-based equality and immutability — `Assert.Equal(TestErrors.HandlerFailure, result.Error)` in `DispatcherTests` relies on it. Factory methods: `Validation` (two overloads — with/without field dictionary; the field overload null-checks), `NotFound`, `Conflict`, `Rule`, `Forbidden`. `ErrorKind` is the closed enum. `CA1716` suppression: `Error` would be a keyword in VB; the solution is C#-only.

Contract: `Code` format `<feature>.<reason>`, lower-case snake; frontends translate codes; `Message` is developer-facing.

## `Common/Entity.cs`
`abstract class Entity` — protected constructor sets `Id = Guid.CreateVersion7()`; `Id` has `private set`. Every concrete entity therefore has an identity the moment it is constructed (before any database involvement), which is why EF is told `ValueGeneratedNever()` for `Centre.Id`. **Tests:** `EntityTests` (distinct ids; `Id.Version == 7`).

## `Common/ITenantOwned.cs`
`interface ITenantOwned { Guid CentreId { get; } }`. Documentation comment: Infrastructure will apply tenant filtering to implementers (Month 2); `Centre` doesn't implement it because it *is* the tenant. **Currently no implementers and no filter.**

## `Centres/SupportedLocale.cs`
`enum SupportedLocale { Ar, En }`. Persisted as lowercase text via a value converter in `CentreConfiguration` (not as the enum's integer). Frontend `Lang` type is `"en" | "ar"` — kept in sync by hand.

## `Centres/Centre.cs`
`public sealed partial class Centre : Entity`.

- **Constants:** `NameMaxLength=120`, `SlugMinLength=3`, `SlugMaxLength=60` — reused by the validator and the EF configuration so limits cannot diverge.
- **Constructors:** private value constructor; private parameterless constructor initialising strings to `string.Empty` "for EF Core materialisation" (silences nullable warnings; EF then overwrites every property).
- **Properties:** `Name`, `Slug`, `TimeZoneId`, `DefaultLocale`, each `{ get; private set; }`.
- **`Create(name, slug, timeZoneId, defaultLocale)`** returns `Result<Centre>`; checks in this order: name null/whitespace → `centre.name_required`; trimmed name > 120 → `centre.name_too_long`; slug null / <3 / >60 / regex mismatch → `centre.slug_invalid`; time-zone blank or `TimeZoneInfo.TryFindSystemTimeZoneById` false → `centre.time_zone_invalid`; else success with the **trimmed** name (slug is not trimmed or lower-cased — it must already match).
- **Slug regex:** `^[a-z0-9]+(?:-[a-z0-9]+)*\z` via `[GeneratedRegex]` (compile-time generated matcher, requires `partial`). Accepts `nile-centre`; rejects `-abc`, `abc--def`, `Bad Slug`; `\z` instead of `$` so a trailing `\n` is rejected.
- Note the length message says "3–60 characters" while the numeric limits come from constants; the name-too-long message hard-codes "120" in the text.

**Tests:** `CentreTests` (valid+trim, empty/whitespace/121/120-char names, four invalid slugs, unknown time zone, Cairo zone). Gap: `SlugMaxLength` boundary (60/61) is tested at the validator level (61 chars) but not in `CentreTests`.

## `AssemblyMarker.cs`
`internal sealed class AssemblyMarker;` (empty type with C# 12 semicolon body). Lets `DependencyRuleTests` do `typeof(TutoringCentre.Domain.AssemblyMarker).Assembly`.
