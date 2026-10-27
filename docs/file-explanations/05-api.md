# Current area file guide

This refreshed area contains 54 application files. [Complete index](INDEX.md). [Current teaching overview](../PROJECT_OVERVIEW3.md). Historical overview documents remain unchanged.

The per-file guides distinguish source-comment intent, recognized declarations and executed verification. Read the complete execution paths in the overview before changing shared boundaries. A source excerpt is evidence of current bytes, not proof that all branches passed a test.

| File | Category | Current guide |
| --- | --- | --- |
| `src/TutoringCentre.Api/AssemblyMarker.cs` | source | [Explanation](src/TutoringCentre.Api/AssemblyMarker.cs.md) |
| `src/TutoringCentre.Api/Auth/ActorMiddleware.cs` | source | [Explanation](src/TutoringCentre.Api/Auth/ActorMiddleware.cs.md) |
| `src/TutoringCentre.Api/Auth/AntiforgeryEndpointFilter.cs` | source | [Explanation](src/TutoringCentre.Api/Auth/AntiforgeryEndpointFilter.cs.md) |
| `src/TutoringCentre.Api/Auth/AntiforgerySetup.cs` | source | [Explanation](src/TutoringCentre.Api/Auth/AntiforgerySetup.cs.md) |
| `src/TutoringCentre.Api/Auth/AntiforgeryTokenResponse.cs` | source | [Explanation](src/TutoringCentre.Api/Auth/AntiforgeryTokenResponse.cs.md) |
| `src/TutoringCentre.Api/Auth/AuthEndpoints.cs` | source | [Explanation](src/TutoringCentre.Api/Auth/AuthEndpoints.cs.md) |
| `src/TutoringCentre.Api/Auth/AuthenticationSetup.cs` | source | [Explanation](src/TutoringCentre.Api/Auth/AuthenticationSetup.cs.md) |
| `src/TutoringCentre.Api/Auth/ChangePasswordRateLimiting.cs` | source | [Explanation](src/TutoringCentre.Api/Auth/ChangePasswordRateLimiting.cs.md) |
| `src/TutoringCentre.Api/Auth/LoginRateLimiting.cs` | source | [Explanation](src/TutoringCentre.Api/Auth/LoginRateLimiting.cs.md) |
| `src/TutoringCentre.Api/Auth/LoginRequest.cs` | source | [Explanation](src/TutoringCentre.Api/Auth/LoginRequest.cs.md) |
| `src/TutoringCentre.Api/Auth/RateLimitRejection.cs` | source | [Explanation](src/TutoringCentre.Api/Auth/RateLimitRejection.cs.md) |
| `src/TutoringCentre.Api/Auth/RequestValidation.cs` | source | [Explanation](src/TutoringCentre.Api/Auth/RequestValidation.cs.md) |
| `src/TutoringCentre.Api/Auth/SelectCentreRequest.cs` | source | [Explanation](src/TutoringCentre.Api/Auth/SelectCentreRequest.cs.md) |
| `src/TutoringCentre.Api/Auth/SessionClaims.cs` | source | [Explanation](src/TutoringCentre.Api/Auth/SessionClaims.cs.md) |
| `src/TutoringCentre.Api/Auth/SessionRevalidationHandler.cs` | source | [Explanation](src/TutoringCentre.Api/Auth/SessionRevalidationHandler.cs.md) |
| `src/TutoringCentre.Api/Auth/SessionValidationOptions.cs` | source | [Explanation](src/TutoringCentre.Api/Auth/SessionValidationOptions.cs.md) |
| `src/TutoringCentre.Api/Cli/AppDbContextFactory.cs` | source | [Explanation](src/TutoringCentre.Api/Cli/AppDbContextFactory.cs.md) |
| `src/TutoringCentre.Api/Cli/DemoModeProvider.cs` | source | [Explanation](src/TutoringCentre.Api/Cli/DemoModeProvider.cs.md) |
| `src/TutoringCentre.Api/Cli/DemoOptions.cs` | source | [Explanation](src/TutoringCentre.Api/Cli/DemoOptions.cs.md) |
| `src/TutoringCentre.Api/Cli/MigrateCommand.cs` | source | [Explanation](src/TutoringCentre.Api/Cli/MigrateCommand.cs.md) |
| `src/TutoringCentre.Api/Cli/ResetPasswordCommand.cs` | source | [Explanation](src/TutoringCentre.Api/Cli/ResetPasswordCommand.cs.md) |
| `src/TutoringCentre.Api/Cli/SeedCommand.cs` | source | [Explanation](src/TutoringCentre.Api/Cli/SeedCommand.cs.md) |
| `src/TutoringCentre.Api/Cli/SeedGate.cs` | source | [Explanation](src/TutoringCentre.Api/Cli/SeedGate.cs.md) |
| `src/TutoringCentre.Api/Endpoints/AuditEndpoints.cs` | source | [Explanation](src/TutoringCentre.Api/Endpoints/AuditEndpoints.cs.md) |
| `src/TutoringCentre.Api/Endpoints/CentreSettingsEndpoints.cs` | source | [Explanation](src/TutoringCentre.Api/Endpoints/CentreSettingsEndpoints.cs.md) |
| `src/TutoringCentre.Api/Endpoints/EnrollmentEndpoints.cs` | source | [Explanation](src/TutoringCentre.Api/Endpoints/EnrollmentEndpoints.cs.md) |
| `src/TutoringCentre.Api/Endpoints/GroupEndpoints.cs` | source | [Explanation](src/TutoringCentre.Api/Endpoints/GroupEndpoints.cs.md) |
| `src/TutoringCentre.Api/Endpoints/ParentEndpoints.cs` | source | [Explanation](src/TutoringCentre.Api/Endpoints/ParentEndpoints.cs.md) |
| `src/TutoringCentre.Api/Endpoints/PlatformEndpoints.cs` | source | [Explanation](src/TutoringCentre.Api/Endpoints/PlatformEndpoints.cs.md) |
| `src/TutoringCentre.Api/Endpoints/ScheduleEndpoints.cs` | source | [Explanation](src/TutoringCentre.Api/Endpoints/ScheduleEndpoints.cs.md) |
| `src/TutoringCentre.Api/Endpoints/StaffEndpoints.cs` | source | [Explanation](src/TutoringCentre.Api/Endpoints/StaffEndpoints.cs.md) |
| `src/TutoringCentre.Api/Endpoints/StudentEndpoints.cs` | source | [Explanation](src/TutoringCentre.Api/Endpoints/StudentEndpoints.cs.md) |
| `src/TutoringCentre.Api/Endpoints/SubjectEndpoints.cs` | source | [Explanation](src/TutoringCentre.Api/Endpoints/SubjectEndpoints.cs.md) |
| `src/TutoringCentre.Api/Endpoints/TeacherEndpoints.cs` | source | [Explanation](src/TutoringCentre.Api/Endpoints/TeacherEndpoints.cs.md) |
| `src/TutoringCentre.Api/Http/CorrelationIdMiddleware.cs` | source | [Explanation](src/TutoringCentre.Api/Http/CorrelationIdMiddleware.cs.md) |
| `src/TutoringCentre.Api/Http/ForwardedHeadersSetup.cs` | source | [Explanation](src/TutoringCentre.Api/Http/ForwardedHeadersSetup.cs.md) |
| `src/TutoringCentre.Api/Http/GlobalExceptionHandler.cs` | source | [Explanation](src/TutoringCentre.Api/Http/GlobalExceptionHandler.cs.md) |
| `src/TutoringCentre.Api/Http/HourMinuteTimeOnlyJsonConverter.cs` | source | [Explanation](src/TutoringCentre.Api/Http/HourMinuteTimeOnlyJsonConverter.cs.md) |
| `src/TutoringCentre.Api/Http/HttpContextKeys.cs` | source | [Explanation](src/TutoringCentre.Api/Http/HttpContextKeys.cs.md) |
| `src/TutoringCentre.Api/Http/ProblemDetailsSetup.cs` | source | [Explanation](src/TutoringCentre.Api/Http/ProblemDetailsSetup.cs.md) |
| `src/TutoringCentre.Api/Http/ProblemResult.cs` | source | [Explanation](src/TutoringCentre.Api/Http/ProblemResult.cs.md) |
| `src/TutoringCentre.Api/Http/ProxyOptions.cs` | source | [Explanation](src/TutoringCentre.Api/Http/ProxyOptions.cs.md) |
| `src/TutoringCentre.Api/Http/ProxyOptionsValidator.cs` | source | [Explanation](src/TutoringCentre.Api/Http/ProxyOptionsValidator.cs.md) |
| `src/TutoringCentre.Api/Http/ResultHttpExtensions.cs` | source | [Explanation](src/TutoringCentre.Api/Http/ResultHttpExtensions.cs.md) |
| `src/TutoringCentre.Api/Http/SecurityHeadersSetup.cs` | source | [Explanation](src/TutoringCentre.Api/Http/SecurityHeadersSetup.cs.md) |
| `src/TutoringCentre.Api/Http/SpaHostingSetup.cs` | source | [Explanation](src/TutoringCentre.Api/Http/SpaHostingSetup.cs.md) |
| `src/TutoringCentre.Api/Http/UtcInstantJsonConverter.cs` | source | [Explanation](src/TutoringCentre.Api/Http/UtcInstantJsonConverter.cs.md) |
| `src/TutoringCentre.Api/Logging/SensitiveDataDestructuringPolicy.cs` | source | [Explanation](src/TutoringCentre.Api/Logging/SensitiveDataDestructuringPolicy.cs.md) |
| `src/TutoringCentre.Api/Program.cs` | source | [Explanation](src/TutoringCentre.Api/Program.cs.md) |
| `src/TutoringCentre.Api/Properties/launchSettings.json` | configuration | [Explanation](src/TutoringCentre.Api/Properties/launchSettings.json.md) |
| `src/TutoringCentre.Api/Telemetry/TelemetrySetup.cs` | source | [Explanation](src/TutoringCentre.Api/Telemetry/TelemetrySetup.cs.md) |
| `src/TutoringCentre.Api/TutoringCentre.Api.csproj` | configuration | [Explanation](src/TutoringCentre.Api/TutoringCentre.Api.csproj.md) |
| `src/TutoringCentre.Api/appsettings.Development.json` | configuration | [Explanation](src/TutoringCentre.Api/appsettings.Development.json.md) |
| `src/TutoringCentre.Api/appsettings.json` | configuration | [Explanation](src/TutoringCentre.Api/appsettings.json.md) |
