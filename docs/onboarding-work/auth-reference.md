## 7.5 Authentication configuration as an executable contract

| Setting or input | Current value/meaning | Source owner | Observable consequence |
| --- | --- | --- | --- |
| Authentication scheme | Cookie scheme | AuthenticationSetup | Sign-in and sign-out address the same ticket handler. |
| Session cookie name | `__Host-tcm.session` | AuthenticationSetup | Browser host-prefix requirements apply. |
| Session HttpOnly | true | AuthenticationSetup | Script reads cannot directly obtain the ticket through document.cookie. |
| Session Secure | Always | AuthenticationSetup | Browser delivery and testing require a compatible secure-origin path. |
| Session SameSite | Lax | AuthenticationSetup | Cross-site navigation behaviour differs from the Strict antiforgery cookie. |
| Session Path | `/` | AuthenticationSetup | The credential matches application paths at the origin. |
| Ticket duration | eight hours, sliding | AuthenticationSetup | Ticket renewal and expiry remain separate from validation-cache reuse. |
| User claim | `sub` | SessionClaimNames | Actor construction requires a parseable user identifier. |
| Stamp claim | `stamp` | SessionClaimNames | Revalidation compares the ticket with account state. |
| Centre/role claims | `centre`, `role`, issued together | SessionPrincipalFactory | A signed-in session can remain deliberately unselected. |
| Key persistence | DbContext-backed key ring | Infrastructure registration | Compatible hosts can use the persisted key material. |
| Antiforgery cookie | `__Host-tcm.xsrf`, Strict | AntiforgerySetup | A separate protected token channel accompanies unsafe API requests. |
| Antiforgery request header | `X-XSRF-TOKEN` | AntiforgerySetup and apiFetch | The application's script explicitly supplies the request token. |
| Login rate policy | configurable fixed window; default ten/minute | LoginRateLimiting | Excess source attempts receive the shared429 contract. |
| Password-change policy | five/minute per user | ChangePasswordRateLimiting | An authenticated user has a separate attempt budget. |
| Account lockout | five failures, fifteen minutes | Infrastructure Identity options | Correct credentials can still be refused during lockout. |
| Password creation policy | length ten, no composition requirements | Infrastructure Identity options | Account creation/password replacement and login shape checks have different jobs. |
| Positive session cache | sixty seconds by default | SessionValidationOptions | Role/status/stamp changes can take a cache interval to be observed. |

## 7.6 Login trace with failure boundaries

The login form first runs its client schema. A blank or syntactically invalid input can be shown immediately without consuming a login request. This is a convenience and request-volume improvement, not proof the server can trust the body. A direct client can bypass every browser field check. Server validation therefore independently checks the transport request before credential verification.

The transport wrapper obtains the antiforgery request token and sends the JSON operation. The browser separately supplies its cookie side. Proxy forwarding reaches the backend with the required scheme. Trusted forwarding, request identifiers, security headers and error middleware run in their configured order. The login endpoint is anonymous by design, but anonymity does not exempt it from its rate policy or group antiforgery filter.

If the source limiter has no permit, rejection happens before endpoint execution. If the antiforgery pair is invalid, the filter returns `auth.csrf_invalid` before credential verification. If JSON cannot bind, the common malformed-request path applies. If shape validation fails after binding, field-keyed validation errors are returned. These are distinct refusal points; a test that exercises only wrong passwords does not establish any of them.

Credential verification then looks up the account. Unknown accounts perform the dummy verification and get the generic credential code. Locked accounts get that same refusal. Wrong passwords for known accounts update failure state and get that same refusal. Correct credentials reset failed-attempt state and return the identifier/stamp through the port. The implementation intends to reduce account enumeration, but its tests of identical bodies do not measure equal timing distributions.

Only after credentials succeed does the endpoint replace the request actor. This matters for re-login from a browser already carrying a ticket: the existing actor cannot simply be set a second time through the ordinary one-time setter. Reauthentication is the explicit edge operation for replacing identity after fresh credential proof. The subsequent profile query therefore reads the newly authenticated account rather than whichever account an older cookie identified.

The profile read returns active memberships and the pending-password flag. Login cannot select a centre before that read because the server needs the relationship and its role. An account with one membership can be selected automatically only when no password change is pending. An account with several memberships needs user choice; an account with no active membership has no centre access to grant. The response and ticket both reflect that selected or unselected state.

The endpoint builds the principal and signs it in after those steps. It recomputes the response permission list for an automatically selected role, because the profile query happened while the actor was still unselected. The frontend refreshes antiforgery state, writes the returned session into the query cache and navigates according to the pending-change/selection/redirect rules. An accepted internal redirect is a navigation choice, not authority to read the target page's API data.

## 7.7 Session transitions, one at a time

**Centre selection.** The caller supplies only the requested centre identifier. The user identity comes from the trusted actor. The handler first requires a staff actor, then refuses pending password change, then asks for an active membership. It does not reveal whether a refused centre is absent, foreign or inactive through different machine outcomes. Only the successful result supplies the centre and role written to the new principal. A failed transition leaves the previous ticket intact. The client refreshes its CSRF token and session read; tenant data cache identity is a separate concern documented in the findings.

**First login.** A newly created account's pending flag blocks auto-selection even with one membership. The cookie proves the account authenticated but carries no centre. The password-change screen is reachable in that intermediate state. Manually calling centre selection also encounters the server-side pending-change check. Academic requests need a selected tenant and therefore remain refused. This design expresses the gate through session/tenant state instead of trusting a frontend-only redirect.

**Password change.** The endpoint validates its transport shape and requires authentication. The adapter rejects an unknown user, the same old/new value, an incorrect current password or policy failure. After a successful Identity password operation it clears the pending flag and requests a stamp update. The current unchecked secondary result is a real limitation, so the flow must not be described as an explicitly atomic two-store-operation transaction. The endpoint reissues this browser's ticket without a centre; other tickets are subject to stamp revalidation after their cache window.

**Role change.** A membership edit updates the role relationship under the staff use case's invariants and persistence version checks. Old tickets may still contain the earlier role. Their next uncached validation compares the stored current role with that claim and rejects a mismatch. The frontend permission array is not a substitute for that check, because a caller can make requests without rendering any menu. The bounded freshness window is an intentional consequence of positive caching, not evidence the role change was ignored.

**Membership deactivation.** The account can still exist and can have active memberships elsewhere. The selected centre's inactive membership makes revalidation fail for that selected ticket. A later fresh profile does not offer the inactive centre. This is why deleting the account is not the same operation: the relationship can be revoked independently of the person's other access and identity data.

**Logout.** The endpoint signs the cookie scheme out. The client then refreshes token state, clears query data and navigates. Those later awaits can fail independently of the server's logout; UI cleanup needs its own tests. The server does not rotate a stamp solely for logout and does not delete a per-ticket session record. A copied ticket's remaining validity is therefore governed by expiry and compared account/membership facts. Describing this as immediate revocation of every copy would be false.

**Operator reset.** This process command has a different trust level from the web endpoint and does not require knowing the old password. Its current remove-then-add implementation has a rejected-replacement risk described in the findings. The command being operator-only does not make partial failure harmless; it changes who can invoke it. A future repair should test preservation of the old credential when replacement validation fails, rather than testing only a successful reset.

## 7.8 Threats, defenses and remaining limits

| Situation | Current defense | Limit that should remain explicit |
| --- | --- | --- |
| Client invents a user identifier | Verified cookie principal and trusted actor translation | Ordinary body fields must never become privileged actor facts. |
| Client invents a selected centre | Active membership gate before ticket replacement | Selection does not replace data-level filters and RLS. |
| Signed-in role lacks capability | Dispatcher permission marker and role map | Teacher record ownership remains another check after broad capability. |
| Another origin induces unsafe browser request | Antiforgery pair/header validation and cookie policy | Same-origin injected script can obtain the readable request token. |
| Browser script attempts direct ticket extraction | HttpOnly session cookie | Such script may still act through the browser's authenticated request channel. |
| Stolen ticket outlives browser logout | Normal expiry and revalidation still apply | Logout itself does not revoke an independent copy. |
| Membership or role changes after issue | Stamp/status/role comparison | Positive per-process cache delays observation. |
| Password spraying from one source | Login partition limit | A fixed window permits boundary bursts and source trust depends on proxy setup. |
| Repeated guesses against one account | Identity lockout | Attackers can also cause account lockout; the tradeoff is not eliminated. |
| Unknown email probing | Uniform refusal and dummy verification work | Exact timing equality was not measured. |
| Cookie continuity across restart | Database-persisted Data Protection keys | Shared storage is not proof of cloud deployment, backup or at-rest key encryption policy. |
| Forgotten endpoint authorization metadata | Authenticated fallback policy | Deliberately anonymous endpoints still require review of their own behavior. |
| Error reveals internals | Generic unexpected-error body and common writer | Server logs and audit access need their own appropriate retention/access policy. |
| Browser displays old centre data | Backend still scopes fresh API operations | Existing client cache keys/transitions need the regression described in section14. |

The four identity-related checks should be tested independently. An anonymous caller can be refused before a missing tenant marker matters. An unselected caller can be refused before a missing permission check matters. A role without any read permission cannot demonstrate that teacher ownership is correct, because its broad denial masks the record-level condition. Likewise, a query filter can mask a missing RLS policy during an ordinary EF request. Negative tests gain meaning by isolating one decision at a time and checking that refused requests leave no unintended data or audit effects.
