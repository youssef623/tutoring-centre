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

#### How it works

At login the endpoint passes a principal to the configured authentication scheme. The framework serializes ticket data, protects it with the configured Data Protection system and writes the scheme's cookie. On a subsequent request, authentication reads the matching cookie, verifies/unprotects the ticket and checks its validity. The configured validation event performs the application's additional database check before the principal is accepted for later middleware.

Browser flags control delivery and script access. HttpOnly prevents ordinary script reads of the cookie value. Secure restricts delivery to secure requests. SameSite changes whether cross-site initiations carry the cookie. Path scopes matching URLs. The `__Host-` prefix has browser requirements, including Secure, root path and no Domain attribute. ASP.NET writes satisfying settings; the browser enforces acceptance/delivery rules.

Sliding expiration allows active sessions to receive renewed tickets according to the framework's renewal policy. It is not a separate refresh-token endpoint and does not remove the need for revocation checks. A ticket can otherwise remain correctly protected while the membership facts that justified its centre or role have changed.

#### How THIS project implements it

`Api/Auth/AuthenticationSetup.cs:18` registers the cookie scheme. The current settings include:

```csharp
options.Cookie.Name = "__Host-tcm.session";
options.Cookie.HttpOnly = true;
options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
options.Cookie.SameSite = SameSiteMode.Lax;
options.Cookie.Path = "/";

options.ExpireTimeSpan = TimeSpan.FromHours(8);
options.SlidingExpiration = true;
```

The same file assigns `OnValidatePrincipal` to `SessionRevalidationHandler.ValidateAsync`. Its login/access-denied redirect events instead set 401/403 statuses. This is a JSON API behind a SPA: silently redirecting a fetch to an HTML login page would hide the status that frontend session recovery needs. Status-code pages can enrich those otherwise empty statuses with the shared error representation.

`Api/Auth/SessionClaims.cs:23` builds the principal. Every ticket receives `sub` and `stamp`. Centre and role are added only together. The factory does not store email, display name or the full permission list in the ticket. The readable profile comes from `/api/me`, and permissions are computed from the current membership role. Minimal claims reduce duplicated profile data and the amount of state whose freshness must be considered.

The persistent key configuration is current source in `Infrastructure/DependencyInjection.cs:119`:

```csharp
services.AddDataProtection().SetApplicationName("TutoringCentre")
    .PersistKeysToDbContext<AppDbContext>();
```

`AppDbContext` implements the required key context and exposes `DataProtectionKeys`. These keys are platform data rather than a tenant-owned academic entity. The key store must remain accessible while issuing and reading tickets, including after a process restart. Keeping the application name stable matters because cryptographic application isolation contributes to which protected data a process can read.

#### Execution example

An owner submits valid login credentials. `AuthEndpoints.LoginAsync` first validates the request and asks the credential port to verify it. The endpoint replaces the request actor with the authenticated staff identity without a centre. It dispatches the profile read and receives active memberships. If there is exactly one and no pending password change, it chooses that membership. The factory builds `sub`, `stamp`, `centre` and `role`, then `SignInAsync` writes the protected cookie. The response also contains the readable `MeDto`.

The browser's next `/api/me` request includes the cookie automatically. Authentication unprotects it and invokes session revalidation. Once accepted, `ActorMiddleware` translates the claims to a scoped `StaffActor`. The profile query uses that actor's user identifier; it does not accept a new user identifier from the browser body. Its response can update displayed names and memberships without the script ever reading the ticket value.

When a teacher selects another active centre, selection verifies the membership before replacing the ticket with that centre and role. A failed selection does not replace the existing cookie. When password change succeeds, the endpoint issues a ticket with the returned stamp and no centre. When logout runs, it asks the scheme to sign out and the browser receives the corresponding cookie-clearing response.

#### Why this project needs it

ADR 0005 records the same-origin staff SPA context and its preference for protected cookies over JavaScript-held bearer tokens. There is no separate third-party API client that requires a browser-managed JWT flow. Centre selection changes session context, making server-issued ticket replacement a natural operation. Database revalidation addresses the limitation that a protected self-contained ticket does not disappear merely because authorization records changed.

The production image also makes key persistence important. A container filesystem is not a durable identity store. A process issuing tickets with one key ring and a restarted process reading another ring would cause avoidable session loss. Persisting the ring through the shared database gives the local production-style stack a concrete persistence mechanism rather than a comment about future storage.

#### Alternatives

An opaque server-side session identifier supports deletion-based revocation but requires a session store and its own availability/scaling policy. A JWT stored in browser local storage is script-readable and brings a different token lifecycle. A JWT in a cookie still needs a CSRF strategy because cookie delivery is automatic. Very short tickets reduce the stale-access window but add renewal traffic and do not by themselves express immediate revocation. An external identity provider changes credential responsibility and requires its own protocol integration; no such integration is registered here.

The repository explicitly explains some alternatives in its authentication ADR. Operational details not stated there should remain interpretations. In particular, persisting key material is not a claim that keys are independently encrypted at rest by a cloud service, that backups are configured or that cloud instances have already been deployed. Those require separate infrastructure evidence.

#### Important details

The cookie lifetime and the positive validation-cache duration are different clocks. The ticket has an eight-hour sliding configuration; the revalidation answer can be reused for sixty seconds by default. A user can therefore have a cryptographically valid ticket whose database authorization has changed, and the cache determines how soon that change is observed.

Logout clears this browser's credential but does not rotate the account stamp. A copied ticket is still governed by expiry and revalidation. The current endpoint therefore implements browser sign-out, not a server-recorded revocation of one particular ticket. Rotating a user's stamp to change that policy could also affect other sessions and should be designed deliberately.

The key ring is sensitive even though it is not the user's password. Access to cryptographic key material and application identity affects who can produce/read protected tickets. Runtime-role grants, database access, backups and operational key retention belong in the review. Sharing the same key store is necessary for compatible instances, but does not by itself establish correct proxy trust, cookie origin, role freshness or tenant isolation.

#### What I should understand before modifying this code

Find every issuer and reader before adding a claim. Update the principal factory, actor translation and revalidation inputs consistently. Keep centre and role paired. Preserve the API's 401/403 behaviour when changing authentication callbacks. Check the browser's `__Host-` requirements before changing flags or path. Verify restart acceptance with the persisted key store rather than relying on a local interactive host's key defaults. Distinguish a successful cookie write from a successful profile read and ensure issuance remains after required authentication/profile work.

<a id="auth-csrf"></a>
## 6.81 Antiforgery token pairs, unsafe methods and one bounded retry

#### What it means

CSRF exploits the browser's automatic credential delivery. A malicious origin can sometimes cause a signed-in browser to make a request to another origin. The target sees the browser's cookies, which identify a user, but the user may not have intended the action. Authentication proves who owns a credential; it does not prove that the current action originated from the application's own interface.

An antiforgery mechanism requires an additional value that the application's script obtains from its own origin and explicitly sends with an unsafe request. A foreign page may induce a request, but ordinary cross-origin restrictions prevent it from simply reading the application's token response. ASP.NET Core provides related cookie and request tokens and validates their relationship. They must not be described as two copies of an identical plaintext secret: the framework generates and validates distinct protected token representations.

Login CSRF matters even before the victim has an existing session. The attack can sign the victim into an attacker-chosen account and cause later data entry in the wrong account. Protecting only endpoints already marked authenticated leaves that variant unaddressed. The current API group includes login in its explicit unsafe-method check.

#### The problem it solves

```csharp
// Hypothetical: a state-changing JSON endpoint with only authentication.
api.MapPost("/session/centre", SelectCentreAsync);
// Cookie verification alone says nothing about the origin's intended action.
```

The repository's group adds a filter that validates the extra proof before calling the endpoint. SameSite cookie policy contributes a browser-level defense, but it is not treated as the sole guarantee. Authentication, same-origin access and antiforgery each establish a different property. A request that is authenticated can still fail antiforgery, and that failure should leave the session and business state untouched.

#### How it works

The application asks `IAntiforgery.GetAndStoreTokens` for a token set. The service writes the cookie side where necessary and returns the request token. The endpoint returns that request token in JSON. The frontend retains its promise/value in memory and attaches it through `X-XSRF-TOKEN` on later unsafe API calls. The browser independently attaches the cookie.

The endpoint filter calls `ValidateRequestAsync`. It either continues the invocation or catches the framework's antiforgery validation exception and returns the stable failure. It does not catch every endpoint fault and relabel it CSRF. Safe methods are exempt according to the filter's explicit set. The mechanism depends on mapping unsafe endpoints through the protected group, not on the endpoint's filename or the mere registration of antiforgery services.

#### How THIS project implements it

`Api/Auth/AntiforgerySetup.cs:15` configures header `X-XSRF-TOKEN` and cookie `__Host-tcm.xsrf`. The cookie is HttpOnly, Secure always, SameSite Strict and root path. `AuthEndpoints.GetAntiforgeryTokenAsync` returns the request token with `Cache-Control: no-store`. That avoids reusing a cached response as though it were a token for whatever identity happens to use the browser next.

`Program.cs` constructs the API group with `AddEndpointFilter<AntiforgeryEndpointFilter>()`. The filter's current safe set is GET, HEAD and OPTIONS. Its failure branch is:

```csharp
catch (AntiforgeryValidationException)
{
    return Error.Forbidden("auth.csrf_invalid",
        "A valid antiforgery token is required for this request.").ToProblemResult();
}
```

The normal call to `next` occurs only after validation succeeds or the method is safe. That position is how the filter prevents side effects rather than checking for an invalid token after a handler has already changed the session.

Frontend implementation belongs to `frontend/src/api/csrf.ts` and `apiFetch.ts`. Generated client functions use the wrapper, so application code does not have to remember the header in every mutation. A recognized `auth.csrf_invalid` response triggers a refresh and exactly one retry. The bounded retry repairs a stale pair without creating an endless request loop. Session-changing flows also refresh the token because the identity context for validation can change.

#### Execution example

The browser loads the login page without a token in memory. Submitting the valid form calls the generated login mutation. The wrapper sees an unsafe method and obtains the antiforgery token, receiving the cookie side and readable request token through their different channels. It sends the JSON login request with the request token header. Rate limiting executes before endpoint filtering; the filter then validates the pair before the credential operation runs.

Successful login changes the session principal. The frontend refreshes the token for subsequent unsafe requests. Later, a stale request token can produce `403 auth.csrf_invalid`. The wrapper fetches a fresh token and repeats the operation once. If the second attempt also fails, the error returns to the caller; it is not silently dismissed as a successful mutation.

The logout failure case is especially useful to reason about. A request missing the header must be refused before `SignOutAsync`. A later `/api/me` should still observe the existing session. Otherwise the API could claim it prevented CSRF while already performing the side effect. The security tests use real token acquisition rather than disabling the filter, so they exercise this order.

#### Why this project needs it

The browser's session credential is automatically attached to requests, and the SPA has several state-changing operations: login/logout, centre selection, password change and academic/admin commands. Group-wide protection makes a newly mapped unsafe API route participate by default. ADR 0005 identifies cookies and login CSRF as the motivation. The repository does not rely on a developer remembering one extra header at every handwritten fetch call.

The API's JSON convention is also relevant. The filter comments explain that JSON minimal endpoints do not receive the same automatic form antiforgery path merely because services were registered. Explicit validation makes the mechanism visible and testable. This is a case where naming a framework feature without identifying the trigger would leave a genuine security gap.

#### Alternatives

A synchronizer token stored with a server-side session uses a different storage model. Origin/Referer checks can add information but need a carefully defined missing-header and proxy policy. A fixed custom header relies on browser cross-origin request restrictions rather than a generated token relationship. SameSite alone does not provide the same explicit proof and has a different navigation/login threat model. Per-endpoint filters are possible but make omission easier than the current group mapping.

#### Important details

CSRF protection assumes the adversary cannot run arbitrary code in the application's own origin. XSS can obtain the same readable request token as the legitimate script and issue requests from that origin. The session cookie remaining HttpOnly limits direct credential reads but does not prevent those actions. Content policy and careful rendering therefore remain relevant.

The antiforgery cookie's Strict setting differs from the session cookie's Lax setting. The former is needed for same-origin script operations, while the session can accompany permitted top-level navigation. Those choices should not be collapsed into one identical option because both names contain cookies. The host prefix and Secure requirement also explain why the API proxy must reach an HTTPS backend in browser workflows.

A retry is safe here only because the recognized failure happens before the endpoint executes. Do not generalize that retry to every network failure: after a lost response, the server may already have committed the action. The wrapper's exact code predicate and one-retry bound are therefore part of correctness, not merely convenience.

#### What I should understand before modifying this code

Map unsafe endpoints on the protected group. Keep the header constant, frontend wrapper and test helper consistent. Refresh token state when changing session identity. Preserve the filter's before-side-effects order and test a refusal followed by a read that proves state stayed unchanged. Do not store the session credential or request token in local storage as an incidental workaround for a stale-token error. Diagnose cookie origin, proxy scheme and pair lifecycle first.

<a id="auth-rate"></a>
## 6.82 Rate limits, account lockout and enumeration resistance

#### What it means

Rate limiting bounds operations per source or identity over time. Account lockout limits repeated credential failures for one account. Enumeration resistance avoids revealing whether a submitted account exists. These defenses overlap but do not replace one another. An IP limit slows spraying many accounts from one source. A per-account lockout slows guessing one account from many sources. Uniform responses prevent the failure API from becoming an account directory.

A fixed window grants a configured number of permits per interval. Once no permit remains, the request is rejected rather than queued. Its boundary can permit bursts across adjacent windows, so a ten-per-minute setting is not a promise of evenly spaced attempts. The application chooses this model explicitly through the ASP.NET rate-limiting configuration.

#### The problem it solves

```csharp
// Hypothetical account-disclosing response.
if (user is null) return Results.NotFound("Email does not exist");
if (!passwordMatches) return Results.Unauthorized();
```

That difference lets an unauthenticated caller discover registered addresses. Returning the same message closes the obvious response distinction, but timing and workload differences can remain. The current credential adapter performs a dummy password verification for unknown users. This is a defense intent and additional work, not a measured proof that all failure timings are equal.

Without request limits, an attacker can issue repeated expensive credential checks even if every answer is generic. Without account protection, rotating sources can continue attacking one account. Without careful test partitioning, the application's own parallel test clients can accidentally consume a shared source budget and create unrelated failures.

#### How it works

An endpoint names a rate policy in metadata. The middleware asks that policy for a partition key, obtains the relevant limiter and either grants execution or invokes the rejection handler. Because this happens before endpoint execution, a rejected login does not reach credential verification. The rejection can communicate a wait interval through `Retry-After` while still using the standard Problem Details body.

Identity owns the account failure counter and lockout end. The adapter looks up a user, checks lockout, verifies the password and either increments failures or resets the counter. Those writes must persist even when authentication returns a failed result, which is why credential verification is not placed inside a normal failed-command rollback. Password creation policy and login input validation also differ: a login attempt must be allowed to submit an existing credential even if a future policy becomes stricter.

#### How THIS project implements it

`Api/Auth/LoginRateLimiting.cs:27` defines defaults of ten permits and one minute. Registration reads `Auth:LoginRateLimit:PermitLimit` and `Auth:LoginRateLimit:WindowSeconds`, builds a fixed-window policy with queue limit zero and uses the shared `RateLimitRejection.HandleAsync`. The ordinary key is `HttpContext.Connection.RemoteIpAddress`, falling back to `unknown`. Trusted forwarded-header middleware runs earlier, so that address is now proxy-aware under the configured trust policy; the rate-limiter's old comment saying this is future work is stale.

The Testing environment alone can use `X-Test-RateLimit-Partition` to vary a test's partition. Development and Production do not honor that header. This is a scoped testing escape hatch, not a supported client mechanism for avoiding limits. E2E uses a higher configured permit budget because many real browser fixture logins originate from one runner; that is distinct from the Testing-only header path.

`ChangePasswordRateLimiting.cs:20` defines a separate five-per-minute policy. It partitions by the signed-in user claim rather than source IP, because the authenticated actor is known for that operation. Both policies use the same rejection delegate. A single `OnRejected` setting belongs to the shared options instance, and assigning the same delegate from both registrations does not create two independent rejection pipelines.

Infrastructure Identity registration sets five failed account attempts and a fifteen-minute lockout, permits lockout for new users, requires unique email and uses a password length of ten without composition requirements. `IdentityAuthenticationService.cs:23` uses an unknown-user dummy check, a locked-user branch, a wrong-password counter increment and a successful reset. All credential refusal branches return `auth.invalid_credentials`.

#### Execution example

An unauthenticated client sends wrong passwords from one address. The login policy consumes permits before the antiforgery filter and credential endpoint. Requests admitted by the limiter can still fail CSRF and do no credential work. A request with valid CSRF reaches the adapter. An unknown email performs the dummy check and returns the generic failure; a known wrong password increments that user's failure counter and returns the same code.

After five wrong passwords for an account, Identity's configured lockout applies. A correct password submitted while still locked out receives the ordinary credential refusal rather than an account-specific explanation. After the source's configured permit window is exhausted, another login receives 429 and the retry guidance without calling the endpoint. These are two independent counters: a source can exhaust its budget across several accounts, and an account can be locked by attempts across several sources.

The self-service password endpoint instead uses the authenticated user partition. Changing source address does not reset that user's policy key. A Testing fixture can assign a dedicated partition so a test of password error semantics is not accidentally a test of a previous fixture's exhausted budget.

#### Why this project needs it

Login is intentionally public and performs expensive credential verification. Staff records eventually authorize access to sensitive centre data, so uniform refusal and bounded attempts are relevant before academic authorization even begins. The repository implements the defenses and tests their response behaviour. It does not explicitly document why the default numbers are ten, five and fifteen minutes; those are current configuration decisions, not externally verified optimal values.

The application also has a practical need to distinguish production policy from test workload. Raising a browser-suite budget in CI should not require editing the production defaults, and a test-only partition header must not become an attacker's switch. Environment gating and separate configuration inputs express those differences.

#### Alternatives

A token bucket or sliding window changes burst behaviour. Exponential backoff per account changes the lockout experience. A reverse-proxy limit can reject requests before the app but needs correct client-address trust and a policy consistent with application identity. MFA, challenge flows and risk-based checks add different factors and user interactions. None should be claimed implemented merely because Identity's user table contains a two-factor-related field.

#### Important details

An account lockout can itself be abused to deny a known user access by intentionally failing their password. Rate limiting reduces one source's throughput but does not eliminate that tradeoff. The repository's generic failure policy hides which refusal occurred from the unauthenticated client; support and operators may need safe server-side evidence without exposing personal data.

The dummy password check does not prove constant-time behaviour. User lookup, lockout branches, counter writes and framework password rehash behaviour can have different cost. Tests comparing response bodies establish response uniformity, not statistical timing indistinguishability. Avoid repeating the source comment's strongest timing statement as an observed guarantee.

Proxy trust affects source partitions. If every request appears to come from the same proxy, unrelated users share a login budget. If arbitrary clients can forge a trusted forwarding header, an attacker may rotate apparent addresses. The current known-network policy must therefore be reviewed with the limiter, not as unrelated hosting boilerplate.

#### What I should understand before modifying this code

Keep failure bodies generic and test unknown, wrong and locked paths. Preserve the expensive unknown-user defense without claiming measured equality. Change limits through the intended configuration and update tests/messages if the contract changes. Do not honor the test header outside Testing. Validate the forwarded-address chain in the actual deployment topology. Keep credential-counter writes outside any rollback policy that would erase failed-attempt evidence.

<a id="auth-revalidation"></a>
## 6.83 Session revalidation, role freshness and the positive cache

#### What it means

A protected ticket proves that the issuing application accepted some identity facts when it issued the ticket. It does not prove those facts remain current. A staff member can lose a membership, change role or change credentials while an old ticket is still cryptographically readable. Revalidation compares selected ticket facts with current account and membership state. Revocation is the resulting refusal of access before normal ticket expiry.

The security stamp is one account-level comparison value. Membership status and role are centre-level facts. A positive cache remembers a recently valid comparison to avoid repeated database reads. Caching trades immediate observation for lower read load. The relevant stale-access bound is therefore the configured cache duration, not only the ticket lifetime.

#### The problem it solves

```csharp
// Hypothetical removal: cryptographic ticket validation only.
options.Events = new CookieAuthenticationEvents();
// No OnValidatePrincipal callback checks changed membership or role.
```

An old ticket could keep its selected role until expiry even after the database says otherwise. Merely updating `/api/me` output does not invalidate the principal used by another API request. Conversely, checking the database on every request may be unnecessarily expensive for a staff SPA that fetches several resources together. The current implementation puts the check at the authentication boundary with a short positive cache.

#### How it works

Cookie authentication invokes the configured validation event after reading a ticket. The callback parses the facts it needs, constructs a cache key and either uses a remembered valid answer or asks Application to validate current state. It accepts only a successful true result. An invalid or malformed ticket is rejected and the cookie scheme is signed out. The request then continues with no accepted identity for protected authorization.

The query happens before actor middleware, so it cannot use the normal current staff actor as its input. It accepts the ticket's user, stamp, centre and role explicitly for this narrowly defined pre-actor purpose. That exception does not justify accepting a body-supplied user identifier in ordinary “my data” requests.

#### How THIS project implements it

`SessionRevalidationHandler.cs:20` reads `sub` and `stamp`, refusing missing/unparseable identity. It parses optional centre and role and includes both in the key. The current key is the tuple of the `session-valid` label, user, stamp, centre and role. A cached answer for an unselected session is therefore not the same key as an answer for a selected-centre ticket, and a different role is not the same key either.

On a miss the callback dispatches `ValidateStaffSessionQuery`. The handler's actual decision is:

```csharp
var isValid = state is not null
    && state.SecurityStamp == query.SecurityStamp
    && state.MembershipActive
    && state.CurrentRole == query.Role;
```

`MembershipReadService.GetSessionStateAsync` reads the user's stamp. With no centre it returns membership-active true and current role null, matching an unselected ticket. With a centre it loads that user's membership and reports active status and current role. The query returns success containing a boolean; invalid identity state is not treated as an unexpected database exception.

Only true answers are inserted into `IMemoryCache`, and only for a positive configured duration. `SessionValidationOptions` defaults to sixty seconds. Tests can set zero so the next request observes a changed record immediately instead of requiring a timed wait. Rejecting calls both `RejectPrincipal` and `SignOutAsync`, which prevents the browser from repeatedly presenting the same refused cookie.

#### Execution example

A teacher logs in, selects Nile and receives a ticket carrying Teacher. The first authenticated request misses the cache, compares stamp/membership/role and stores a valid answer. An owner changes the membership role. A request arriving while the positive entry is live may still use that answer. After the entry expires, the callback queries current state, sees a different role and rejects the ticket.

The subsequent actor middleware no longer constructs an accepted staff actor from that ticket. The protected endpoint fails authorization with 401. A normal frontend request receiving an unauthenticated error reaches the shared recovery logic, clears cached data and goes to login. The session query has its own convention: a 401 becomes a signed-out null value, which route guards understand. These client paths expose the same server event through different query semantics.

Membership deactivation follows the same shape through the active-status comparison. Password changes cause a stamp mismatch. An unselected session has no centre membership to compare but still requires the account stamp and null role semantics. All of these cases are distinct from a failed database query: a database fault propagates through normal error handling rather than being deliberately cached as a valid session.

#### Why this project needs it

Staff lifecycle and role-based access now exist as product features. A role change must affect more than labels on the next profile screen. The dispatcher bases capability checks on the actor role, so freshness of that role is part of access correctness. The current validation closes the older implementation's role-staleness gap and works with self-contained tickets without adding a separate session table.

#### Alternatives

A server-side session record can be deleted for per-session revocation. A distributed cache can coordinate validation/invalidation across instances. Shorter ticket expiry limits exposure without the same lookup, but changes user experience and renewal policy. Pushing invalidation events can reduce delay but requires another reliable delivery path. None is automatically present because `IMemoryCache` exists; that cache belongs to one process.

#### Important details

“Invalid is not cached” does not mean revocation is never temporarily hidden. A prior valid answer remains usable until its duration expires. The correct statement is that invalid results do not create an additional negative/valid cache entry and a failed uncached comparison is refused. The cache key including the stamp means a newly issued ticket with another stamp is validated independently.

The duration is an upper bound on ordinary cache reuse, not a promise of synchronized cross-process observation. Several instances can each have a positive entry with different creation times. The database key ring lets them read compatible cookies; it does not share their in-memory cache. Correct multi-instance reasoning needs both mechanisms.

Revalidation also differs from logout. It responds to changed compared facts. The current logout does not change those facts and therefore does not revoke an independent copy of the ticket. Describing all sign-out and revocation cases as identical would obscure that limit.

#### What I should understand before modifying this code

Add new authority-bearing claims only with a freshness policy. If a new permission fact is not derived from the role map, decide whether validation must compare it. Update the cache key when its inputs change. Keep test cache duration explicit. Test role change, membership deactivation, stamp change and malformed claims separately. Do not move this query after actor construction without reconsidering which identity is trusted during the check.

<a id="auth-identity"></a>
## 6.84 Identity users, centre memberships and account lifecycle

#### What it means

An identity account answers who a person is and how credentials are verified. A membership answers which centre the account belongs to, in which role and with which status. Those are separate relationships. A person working at two centres should not require two unrelated credential identities merely to express different access. Removing one membership should not erase the user's identity everywhere.

A relational link table can carry data about the relationship itself. Here membership role and status belong to the user-centre pair. A global role on the user cannot express that meaning. The academic Parent and Teacher records are also distinct concepts: a contact person can exist without a login, and a teacher record can be linked to a staff account through an explicit operation rather than being assumed identical to it.

#### The problem it solves

```csharp
// Hypothetical global authorization model, not this repository's user type.
class User { public Guid CentreId { get; set; } public string Role { get; set; } }
```

That shape gives the user one centre and one role. Duplicating the account for another centre duplicates credentials and profile management. Storing a list of roles without the associated centre still loses which role applies where. The current membership entity gives the relationship its own identity, lifecycle and database constraints.

#### How it works

Infrastructure uses ASP.NET Core Identity's user context and `UserManager` for credential and account storage mechanics. Domain owns membership invariant methods independently of Identity classes. Application ports expose profile, account and membership capabilities through DTOs/results. EF configurations map those two families into the identity schema, including the user-centre foreign keys and unique relationship index.

Memberships are intentionally not filtered by the selected tenant in the same way as academic rows. Login must discover the user's centres before any centre is selected. Those reads therefore use an explicit trusted user identity and active membership predicates. This is a justified platform identity path, not a blanket exemption for every record containing `CentreId`.

#### How THIS project implements it

`Infrastructure/Identity/ApplicationUser.cs:10` derives from `IdentityUser<Guid>`, assigns a UUIDv7 identifier and adds DisplayName, PreferredLocale and MustChangePassword. It is an Infrastructure persistence model. Domain does not depend on Identity password stores to construct a membership.

`Domain/Identity/Membership.cs:11` has private constructors and private setters. `Create` rejects empty user and centre identifiers and initializes Active status. `ChangeRole` refuses an inactive membership or the same role. `Deactivate` and `Activate` refuse repeated target states. These methods return normal domain results, allowing handlers to distinguish expected refusals from storage faults.

The context inherits `IdentityUserContext<ApplicationUser, Guid>`, not the role-table context. Its mapping calls Identity's base setup before the project's configurations. Global Identity roles are not the source of centre permission decisions. `RolePermissions` maps the Domain membership role to capability keys. That is why registering a global Identity role manager would not automatically integrate a new staff role with the existing dispatcher.

`MembershipReadService` uses non-tracking projections. Profile reads select user fields, active memberships and centre name/slug/time zone. Active membership lookup filters the user, centre and Active status. Session-state lookup additionally reports the current role. `GetMyMembershipsHandler` derives permissions from the active membership and returns no permissions when no active selected membership remains.

#### Execution example

The seeded two-centre teacher authenticates once. Profile reads find two active memberships. Login issues an unselected ticket instead of guessing which centre the person intends to manage. The centre picker shows both choices. Selecting one dispatches the gate, which verifies the active relationship and returns the role for that centre. That returned role becomes ticket/actor context for permission decisions.

An owner later changes a staff role through the lifecycle use case. The handler operates on the membership relationship, not a global `ApplicationUser.Role` property. Persistence checks version/conflicts and audit history records permitted changes in the command transaction. Revalidation compares the role in the old ticket with the relationship's new role and eventually rejects the stale session.

A newly created staff account can be marked for password change. Login then withholds automatic centre selection even for one membership. `GetActiveMembershipHandler` checks the flag before allowing selection, so manually posting the selection endpoint does not bypass the UI gate. Successful self-service password handling clears the flag through the Identity path, with the unchecked secondary result noted separately in the findings rather than being described as an explicit all-or-nothing transaction.

#### Why this project needs it

The demo data and UI model staff working in more than one centre. Product authorization is per membership. Teacher ownership also connects the selected centre's teacher record to a staff account. Keeping credential identity separate from academic identity and centre relationships avoids forcing all of those concerns into one user row.

It also improves boundary clarity. A handler requesting a staff-directory fact does not need password hashes or lockout columns. A profile DTO does not expose `UserManager`. A Domain membership test can exercise state transitions without a database or authentication framework. These are concrete capabilities enabled by separating the models.

#### Alternatives

Global Identity roles are useful for application-wide access but do not express this centre-local relationship without additional modelling. Encoding every membership in account claims duplicates relationship data and complicates revocation. Deleting a user to remove centre access affects all memberships and loses identity history. A fully custom credential store would give control but also require owning password/security mechanics that this project delegates to Identity.

#### Important details

The user-centre pair's uniqueness belongs in the database as well as any friendly precheck. Two racing requests can both see no membership before saving. Referential integrity and restrictive deletion protect relationships independently of handlers. Do not assume that an academic teacher's existence implies an active eligible staff account, or that a staff role automatically creates a teacher record.

Private setters protect ordinary application mutation paths, but EF materialization still builds objects from stored rows. Database constraints therefore remain valuable against invalid writes made outside factory methods. Similarly, storing a status enum is not enough to enforce visibility: each read path must apply the appropriate active-status/ownership rule.

The PreferredLocale field and current frontend language preference are related but distinct state. Returning a preference does not establish that every browser applies it. Read language initialization and session handlers before claiming that changing a user profile will immediately change the document direction.

#### What I should understand before modifying this code

Decide whether a new fact belongs to the account, membership or academic record. Keep credential types inside their allowed layer. Add role values consistently across Domain, converters/check constraints, permission map, contract and translations. Use membership methods for transitions. Verify centre-local uniqueness, staff eligibility, version conflicts and revocation effects with separate tests rather than testing only a successful creation.

<a id="auth-authorization"></a>
## 6.85 Fail-closed authorization, actor translation and the tenant gate

#### What it means

Authentication establishes identity. Authorization decides whether that identity may perform an action. Tenant selection establishes which customer's data is in context. Ownership can further restrict which records within that tenant are available. A request can pass one decision and fail another. A signed-in teacher is authenticated; it still lacks staff-management permission and cannot read another teacher's group merely because both belong to one centre.

Fail-closed means missing an explicit opt-in leaves access restricted. The API's fallback policy requires an authenticated principal unless the endpoint allows anonymous access. Application request markers then declare additional prerequisites shared across delivery mechanisms. A fallback HTTP policy is not a substitute for those Application checks because operator/test callers can dispatch requests without passing through HTTP middleware.

#### The problem it solves

```csharp
// Hypothetical: accepting authority from the request body.
var actor = new StaffActor(body.UserId, body.CentreId, StaffRole.Owner);
```

That shape lets the caller choose who they are and what role they hold. The current actor comes from trusted credential/session processing. A centre identifier supplied for selection is a requested transition, not accepted authority. The gate verifies active membership before replacing the ticket. Ordinary tenant requests take the active centre from the actor instead of trusting a separate per-request owner flag.

An omission-prone route scheme has another failure mode: a developer adds an endpoint and forgets to protect it. The fallback policy makes that endpoint authenticated by default. Explicit anonymous fallbacks also matter because an unknown path should return a route error, not unexpectedly expose information about whether a caller is signed in.

#### How it works

Routing selects an endpoint and its metadata. Authentication establishes the principal. `ActorMiddleware` parses a valid claim combination and populates a scoped Application actor. Authorization applies endpoint requirements or the fallback authenticated policy. An Application dispatch then validates shape, checks tenant scope markers and checks required capability markers before creating its transaction.

Tenant/permission markers do not perform record ownership checks by themselves. A request can be allowed to view groups while its read adapter must still narrow records to the linked teacher. RLS enforces centre boundaries at storage, while the teacher-scope path handles the narrower subject. The rules cooperate rather than competing as several names for the same guard.

#### How THIS project implements it

`AuthenticationSetup.cs:48` sets an authenticated fallback policy. Anonymous routes opt out deliberately. `ActorMiddleware.cs:19` acts only when the principal is authenticated and `TryBuildActor` succeeds. The parser requires a user GUID. Neither centre nor role means a valid unselected staff actor; both valid values mean a selected actor. A partial pair fails actor construction. The holder starts anonymous and exposes a read-only view to handlers.

Login is a sanctioned identity replacement within the same request: a request might arrive carrying an older ticket while submitting newly verified credentials. `CurrentActorContext.Reauthenticate` replaces that request actor instead of calling the one-time setter again. This is not a general-purpose handler API for escalating identity.

`GetActiveMembershipHandler.cs:20` refuses non-staff callers. It then checks `MustChangePasswordAsync`; a pending change returns `auth.password_change_required`. Only then does it request the active membership. A missing centre, a foreign membership and an inactive membership share `tenant.no_membership`. This avoids turning centre selection into an existence probe. The endpoint issues no new cookie on those failure results.

The permission decision is current dispatcher source:

```csharp
var allowed = actor switch
{
    SystemActor => true,
    StaffActor { Role: { } role } => RolePermissions.Holds(role, permission),
    _ => false,
};
```

It runs only for requests declaring `IRequirePermission`; architecture rules check declaration conventions in scoped modules. The tenant check independently runs for `ITenantScoped`. Unknown or absent role state does not accidentally become owner access. Denial logs use identifiers, capability and request type rather than dumping profile/credential data.

#### Execution example

A signed-in secretary with Nile selected requests the staff page. Frontend permission guards show no access rather than mounting staff data hooks. If the caller bypasses the frontend and sends the API request, HTTP authentication succeeds but the Application permission check refuses staff capability before a transaction opens. UI and server therefore give different kinds of protection, with the server remaining authoritative.

The same user requests student data. The role holds that capability, the tenant is selected and the command/query reaches persistence with Nile's transaction setting. Supplying a Maadi record identifier does not change the active tenant. The EF filter and RLS restrict visibility, and the handler returns the scoped not-found outcome rather than treating the foreign identifier as an authorized tenant switch.

A teacher requests group data. The role holds GroupsView, but the read service also applies the teacher-scope provider's linked-teacher result. No linked teacher should not silently mean all groups. A valid linked teacher sees the appropriate group subset for this centre. The same staff user choosing another centre can have a different teacher record and group set. Permission names alone do not encode that record list.

#### Why this project needs it

The application has multiple roles, multiple centres per user and teacher-specific academic views. Those requirements cannot be represented safely by one `IsAuthenticated` check or a frontend menu. Central dispatch checks make omission less likely across many handlers, while trusted actor translation keeps Application independent of ASP.NET claim parsing. The first-login gate also reuses the centre-selection boundary instead of adding a loosely coordinated second check to every academic endpoint.

#### Alternatives

Per-endpoint role attributes can work but duplicate policy across callers and leave omissions to review. Framework authorization handlers could evaluate capability policies at the HTTP edge, but non-HTTP callers still need Application rules. Passing trusted identity as an explicit internal parameter is possible, but taking it directly from the public DTO is not the same design. An ambient HTTP context inside handlers would avoid an actor type while coupling use cases to transport and scattering claim interpretation.

#### Important details

A malformed Application actor does not by itself rewrite `HttpContext.User`. The parser's comments express a fail-safe intent, but HTTP authorization evaluates its principal and Application prerequisites evaluate the actor. Review both when adding a new pre-actor or exceptional route; do not claim that every parser failure mechanically causes the same HTTP challenge at that exact line.

System actors are trusted process identities, not an Internet-facing superuser role that a request can name. A system actor with no centre can still fail a tenant-scoped prerequisite before its permission exemption matters. This is why the two checks are separate and why the pipeline order must be read in full.

The frontend's permission array can be fresher than a cached ticket role or vice versa during a transition. A server refusal remains valid even when a button was visible. Recovery should refresh the relevant session/data instead of treating UI visibility as proof that the API is wrong. Centre changes additionally require a cache-identity strategy, as the dedicated finding explains.

#### What I should understand before modifying this code

Identify identity, tenant, capability and ownership requirements separately. Declare the right request markers and keep architecture checks meaningful. Never source a privileged actor from public body fields. Preserve uniform failed centre selection. Test anonymous, unselected, permitted, denied, wrong-centre and wrong-teacher cases independently, including absence of writes/audit rows on refusal. Keep new anonymous routes deliberate and document their rationale.
