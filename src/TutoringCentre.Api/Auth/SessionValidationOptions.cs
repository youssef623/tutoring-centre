namespace TutoringCentre.Api.Auth;

/// <summary>Bound from configuration key SessionValidation:CacheDuration (e.g. "00:01:00"). Zero disables caching.</summary>
public sealed class SessionValidationOptions
{
    public static readonly TimeSpan DefaultCacheDuration = TimeSpan.FromSeconds(60);

    public TimeSpan CacheDuration { get; set; } = DefaultCacheDuration;
}
