# tutoring-centre

## Architecture

```mermaid
flowchart TD
    Api["TutoringCentre.Api"]
    Application["TutoringCentre.Application"]
    Infrastructure["TutoringCentre.Infrastructure"]
    Domain["TutoringCentre.Domain"]

    Application --> Domain
    Infrastructure --> Application
    Infrastructure --> Domain
    Api --> Application
    Api -.-> Infrastructure
```

Domain has no dependencies. Application depends only on Domain. Infrastructure implements Application's interfaces (Dependency Inversion Principle), so it depends on Application and Domain. Api references Infrastructure only to register it at startup in the composition root (dashed arrow) — that reference must never leak into endpoint code.