# `docs/onboarding-work/auth-reference.md`

## Purpose

Documentation covering 7.5 Authentication configuration as an executable contract; 7.6 Login trace with failure boundaries; 7.7 Session transitions, one at a time; 7.8 Threats, defenses and remaining limits. Treat historical claims according to the revision described in the file.

## Where it fits

Category: **documentation**. [Open current file](../../../onboarding-work/auth-reference.md). [Complete file index](../../INDEX.md). [Current teaching overview](../../../PROJECT_OVERVIEW3.md).

The reviewed file has 75 lines and 12,350 bytes. Source hash: `427dbad02adc81f54637ccdc20910fe071f9f3f5318232a51d274f789b8409f5`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

No import or project-reference declaration was recognized by the structural extractor. This does not prove the file has no consumers or dependencies.

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 62](../../../onboarding-work/auth-reference.md#L62): `| Signed-in role lacks capability | Dispatcher permission marker and role map | Teacher record ownership remains another check after broad capability. |`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```text
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
```

The excerpt stops at line 24; use the original file for the remaining 51 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
