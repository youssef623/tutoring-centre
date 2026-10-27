# `Caddyfile`

## Purpose

Current Docker/infrastructure file. The source excerpt and structural map below expose its content without inferring behaviour from its filename.

## Where it fits

Category: **Docker/infrastructure**. [Open current file](../../Caddyfile). [Complete file index](INDEX.md). [Current teaching overview](../PROJECT_OVERVIEW3.md).

The reviewed file has 8 lines and 491 bytes. Source hash: `f4f07c253ffc60ffa26dc2e1d4dc7b1b6a2dae951e68b4e841a90fff467d088a`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

No import or project-reference declaration was recognized by the structural extractor. This does not prove the file has no consumers or dependencies.

## Walkthrough and declaration map

- Line 1: Local HTTPS reverse proxy (Task 34.8): terminates TLS with Caddy's own locally-trusted certificate authority
- Line 2: (the "tls internal" directive — no real certificate needed for this local stack) and forwards to the app
- Line 3: container, setting X-Forwarded-For and X-Forwarded-Proto automatically (Caddy's own reverse_proxy default),
- Line 4: which the app trusts only because Proxy:KnownNetworks names this Compose network (compose.prod.yaml).

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```text
# Local HTTPS reverse proxy (Task 34.8): terminates TLS with Caddy's own locally-trusted certificate authority
# (the "tls internal" directive — no real certificate needed for this local stack) and forwards to the app
# container, setting X-Forwarded-For and X-Forwarded-Proto automatically (Caddy's own reverse_proxy default),
# which the app trusts only because Proxy:KnownNetworks names this Compose network (compose.prod.yaml).
localhost:8443 {
	tls internal
	reverse_proxy app:8080
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
