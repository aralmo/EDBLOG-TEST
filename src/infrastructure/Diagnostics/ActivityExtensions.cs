using System.Diagnostics;

public static class ActivityExtensions
{
    public static Activity? AddException(this Activity? activity, Exception ex, string? message)
    {
        return activity?.AddTag("event", "error")
            .AddTag("error", "true")
            .AddTag("error.kind", ex.GetType().Name)
            .AddTag("error.message", message ?? ex.Message)
            .AddTag("error.stack", ex.StackTrace);
    }
}