# ADR 0009: Azure Container Apps and OIDC

- **Status:** Accepted — design only; not yet provisioned. No Azure subscription exists for this project as of
  2026-10-11 (Tasks 35.1–35.5 are undone, not done against a different target); see
  `docs/architecture/deployment.md` for exactly what is built versus designed, and `later.md` for the trigger
  to revisit this status.
- **Date:** 2026-10-11

## Context

The application needs to run on a public URL, deployed automatically from `main`, with migrations applied by
the pipeline — never by the running web process (Task 34.7 already rejected startup migrations: several
instances racing to apply the same migration, with no review step, is a correctness risk a container platform
with a separate job primitive avoids entirely). The pipeline needs credentials to push an image and update the
running service, and those credentials must not be a long-lived secret sitting in GitHub waiting to leak or
need rotation.

## Alternatives considered

- **Azure App Service** (a PaaS web app, deployed by pushing code or an image) is simpler to provision than a
  full Container Apps environment and well-trodden for a single web process. It has no first-class "job"
  primitive of its own for a one-shot task like `migrate`: the common pattern is either a startup script (the
  exact race Task 34.7 already rejected) or a separate, hand-rolled mechanism (a WebJob, or a second App
  Service tier doing nothing but running a command) bolted on rather than native to the platform. It also
  supports OIDC federated credentials for GitHub Actions the same way Container Apps does, so that half of
  this decision is not what distinguishes the two.
- **A virtual machine** (deploy the container, or the published binaries, directly onto a VM this project
  manages) gives full control over the host and no platform-specific abstractions to learn, at the cost of
  owning everything the platform would otherwise own: OS patching, the reverse proxy and its TLS certificate
  (reinventing Caddy's job, Task 34.8, at the infrastructure layer instead of letting a managed ingress do
  it), scaling, and a bespoke way to run `migrate` as a one-shot step rather than a built-in job type. For a
  single small app, this is meaningfully more operational surface for no corresponding benefit.
- **Migrations run at application startup** (already rejected, Task 34.7) — several instances racing to apply
  the same migration concurrently, with no review gate between "code is ready" and "schema changes," is a
  correctness risk the moment there is more than one running instance, which any platform with rolling
  deployments or multiple replicas eventually has.
- **A stored service-principal client secret** (a long-lived Azure credential saved as a GitHub Actions
  secret, authenticating the normal way) works with every Azure compute option equally, including both
  alternatives above. It is rejected regardless of which compute platform is chosen: a secret saved in GitHub
  has to be rotated on a schedule, can leak through a workflow log or a compromised dependency, and is valid
  from anywhere, not just from this repository's own `production` environment. OIDC's federated credential
  trusts a cryptographically signed, short-lived token GitHub issues on the pipeline's behalf, scoped to a
  specific repository and environment (Task 35.3), with nothing stored to leak or rotate.

## Decision

Azure Container Apps (the app, plus two Container Apps Jobs — `migrate` and `seed` — reusing the same image)
behind Azure Container Registry, Azure Database for PostgreSQL Flexible Server, and Application Insights for
telemetry (Task 35.1's scope note), authenticated from GitHub Actions via a federated OIDC credential scoped
to this repository's `production` environment (Task 35.3) — never a stored client secret. Migrations and the
demo seed run as separate jobs, triggered by the pipeline, with the owner database role; the running app
container never holds that connection (Task 34.3/35.2).

## Consequences

Container Apps' own Jobs primitive makes "migrate before the new revision serves traffic" (deploy step 3,
`docs/architecture/deployment.md`) a platform-native concept rather than a workaround, matching the ordering
this project already committed to at the code level (Task 34.7). OIDC means there is no Azure credential in
GitHub to rotate or leak — the trade is a more involved one-time setup (a federated credential, scoped role
assignments, Task 35.3) versus pasting a secret into GitHub once.

The environment carries real monthly cost and more moving parts (a Container Apps environment, a registry, a
managed PostgreSQL server, an Application Insights resource) than App Service's single-resource footprint —
accepted because the job primitive and the OIDC trust model are exactly what Tasks 34.7's and 35.3's own
decisions already require, not a convenience chosen first and justified after.

This decision is currently unexecuted. It records *what* was chosen and *why*, before Tasks 35.1–35.5
provision it — the reverse of the usual order, made necessary by this project having stopped at the design
boundary (no Azure subscription) rather than past it. If a future session revisits compute choice before
provisioning (a cost constraint, a platform limitation discovered during 35.1), that is a new ADR, not an edit
to this one.
