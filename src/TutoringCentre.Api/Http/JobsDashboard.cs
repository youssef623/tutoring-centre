using Hangfire;

namespace TutoringCentre.Api.Http;

public static class JobsDashboard
{
    /// <summary>Every dashboard route, including assets and actions, uses the platform operator policy.</summary>
    public static void MapJobsDashboard(this WebApplication app, string path)
    {
        ArgumentNullException.ThrowIfNull(app);
        var readOnly = !app.Environment.IsDevelopment();
        var dashboard = app.MapHangfireDashboard(path, new DashboardOptions
        {
            // Endpoint authorization replaces Hangfire's default local-request shortcut.
            Authorization = [],
            IsReadOnlyFunc = _ => readOnly,
        }).RequireAuthorization("PlatformOperator");
        dashboard.Add(endpoint =>
        {
            var next = endpoint.RequestDelegate!;
            endpoint.RequestDelegate = async context =>
            {
                if (readOnly && !HttpMethods.IsGet(context.Request.Method) && !HttpMethods.IsHead(context.Request.Method))
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    return;
                }
                await next(context);
            };
        });
    }
}
