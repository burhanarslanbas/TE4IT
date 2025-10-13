namespace TE4IT.API.Configuration.Constants;

public static class RateLimitPolicies
{
    public const string FixedRefresh = "fixed-refresh";
    
    public static class FixedRefreshPolicy
    {
        public static readonly TimeSpan Window = TimeSpan.FromSeconds(10);
        public const int PermitLimit = 5;
        public const int QueueLimit = 0;
    }
}
