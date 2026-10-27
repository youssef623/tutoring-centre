# `docs/onboarding-work/auth-depth.md`

## Purpose

Documentation covering 6.80 Cookie tickets, browser policy and the persistent key ring; 6.81 Antiforgery token pairs, unsafe methods and one bounded retry; 6.82 Rate limits, account lockout and enumeration resistance; 6.83 Session revalidation, role freshness and the positive cache; 6.84 Identity users, centre memberships and account lifecycle. Treat historical claims according to the revision described in the file.

## Where it fits

Category: **documentation**. [Open current file](../../../onboarding-work/auth-depth.md). [Complete file index](../../INDEX.md). [Current teaching overview](../../../PROJECT_OVERVIEW3.md).

The reviewed file has 443 lines and 53,091 bytes. Source hash: `700bf44bf18c9629e111d63be1bd4abefb5f3b148d94355e54978217450b9c0f`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

No import or project-reference declaration was recognized by the structural extractor. This does not prove the file has no consumers or dependencies.

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 276](../../../onboarding-work/auth-depth.md#L276): `Only true answers are inserted into IMemoryCache, and only for a positive configured duration. SessionValidationOptions defaults to sixty seconds. Tests can set zero so the next request observes a changed record immediately instead of requiring a timed wait. Rejecting calls both RejectPrincipal and SignOutAsync, which prevents the browser from repeatedly presenting the same refused cookie.`
- [Line 292](../../../onboarding-work/auth-depth.md#L292): `A server-side session record can be deleted for per-session revocation. A distributed cache can coordinate validation/invalidation across instances. Shorter ticket expiry limits exposure without the same lookup, but changes user experience and renewal policy. Pushing invalidation events can reduce delay but requires another reliable delivery path. None is automatically present because IMemoryCache exists; that cache belongs to one process.`
- [Line 313](../../../onboarding-work/auth-depth.md#L313): `A relational link table can carry data about the relationship itself. Here membership role and status belong to the user-centre pair. A global role on the user cannot express that meaning. The academic Parent and Teacher records are also distinct concepts: a contact person can exist without a login, and a teacher record can be linked to a staff account through an explicit operation rather than being assumed identical to it.`
- [Line 319](../../../onboarding-work/auth-depth.md#L319): `class User { public Guid CentreId { get; set; } public string Role { get; set; } }`
- [Line 328](../../../onboarding-work/auth-depth.md#L328): `Memberships are intentionally not filtered by the selected tenant in the same way as academic rows. Login must discover the user's centres before any centre is selected. Those reads therefore use an explicit trusted user identity and active membership predicates. This is a justified platform identity path, not a blanket exemption for every record containing CentreId.`
- [Line 350](../../../onboarding-work/auth-depth.md#L350): `The demo data and UI model staff working in more than one centre. Product authorization is per membership. Teacher ownership also connects the selected centre's teacher record to a staff account. Keeping credential identity separate from academic identity and centre relationships avoids forcing all of those concerns into one user row.`
- [Line 362](../../../onboarding-work/auth-depth.md#L362): `Private setters protect ordinary application mutation paths, but EF materialization still builds objects from stored rows. Database constraints therefore remain valuable against invalid writes made outside factory methods. Similarly, storing a status enum is not enough to enforce visibility: each read path must apply the appropriate active-status/ownership rule.`
- [Line 394](../../../onboarding-work/auth-depth.md#L394): `Tenant/permission markers do not perform record ownership checks by themselves. A request can be allowed to view groups while its read adapter must still narrow records to the linked teacher. RLS enforces centre boundaries at storage, while the teacher-scope path handles the narrower subject. The rules cooperate rather than competing as several names for the same guard.`
- [Line 421](../../../onboarding-work/auth-depth.md#L421): `The same user requests student data. The role holds that capability, the tenant is selected and the command/query reaches persistence with Nile's transaction setting. Supplying a Maadi record identifier does not change the active tenant. The EF filter and RLS restrict visibility, and the handler returns the scoped not-found outcome rather than treating the foreign identifier as an authorized tenant switch.`
- [Line 423](../../../onboarding-work/auth-depth.md#L423): `A teacher requests group data. The role holds GroupsView, but the read service also applies the teacher-scope provider's linked-teacher result. No linked teacher should not silently mean all groups. A valid linked teacher sees the appropriate group subset for this centre. The same staff user choosing another centre can have a different teacher record and group set. Permission names alone do not encode that record list.`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

````text
<a id="auth-cookie"></a>
## 6.80 Cookie tickets, browser policy and the persistent key ring

#### What it means

HTTP requests do not intrinsically remember a previous login. A session mechanism lets a later request carry evidence established during authentication. A cookie is a browser-managed header value: a response asks the browser to store it, and the browser later includes it on matching requests. A cookie is a transport/storage mechanism, not automatically a secure identity. Its value must be produced and verified through a trustworthy scheme.

Two common designs put different things in that cookie. A server-side session uses an opaque identifier whose associated identity is stored in a database or cache. A protected-ticket design places the identity facts and ticket metadata in the protected value itself. The current application configures ASP.NET Core cookie authentication, builds a `ClaimsPrincipal` and calls `SignInAsync`. Data Protection supplies the cryptographic protection used by that framework integration. There is no application session table that the logout endpoint deletes.

A claim is a named fact, such as the authenticated user's identifier or selected centre. A principal contains an identity and claims. The principal is not the profile DTO returned to React, and a profile response cannot authenticate the next request merely because it contains the same user identifier. The browser's protected cookie and the server's verification of it carry that authority.

#### The problem it solves

Without verified session evidence, a server might trust a claimed identity from arbitrary input:

```csharp
// Hypothetical insecure design, not current repository code.
var userId = Guid.Parse(httpContext.Request.Headers["X-User-Id"]!);
return Results.Ok(await ReadPrivateRecords(userId));
```

The sender can change that header. Repeating a password on every request is a different undesirable alternative: every action becomes another credential-handling operation, and the browser must retain credentials for later use. A session lets the server authenticate once and issue a different bounded credential for subsequent requests.

Keeping that credential out of JavaScript-readable storage limits extraction through injected script. It does not prevent injected same-origin code from using the signed-in browser to act. The cookie flags, antiforgery mechanism, content policy, authorization and data isolation protect different parts of the threat model. Saying only “cookies are secure” would conceal those dependencies.
````

The excerpt stops at line 24; use the original file for the remaining 419 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
