namespace TE4IT.API.Configuration.Constants;

public static class RateLimitPolicies
{
    public const string FixedRefresh = "fixed-refresh";
    public const string AuthPolicy = "auth-policy";           // ✅ Login endpoint'i için
    public const string ApiPolicy = "api-policy";             // ✅ Genel API için
    
    public static class FixedRefreshPolicy
    {
        public static readonly TimeSpan Window = TimeSpan.FromSeconds(10);
        public const int PermitLimit = 5;
        public const int QueueLimit = 0;
    }
    
    // ✅ Login endpoint'i için sıkı rate limiting
    public static class AuthPolicyConfig
    {
        public static readonly TimeSpan Window = TimeSpan.FromMinutes(15);  // 15 dakika pencere
        public const int PermitLimit = 5;                                  // 5 deneme
        public const int QueueLimit = 0;                                    // Queue yok
    }
    
    // ✅ Genel API için rate limiting
    public static class ApiPolicyConfig
    {
        public static readonly TimeSpan Window = TimeSpan.FromMinutes(1);   // 1 dakika pencere
        public const int PermitLimit = 100;                                 // 100 istek
        public const int QueueLimit = 10;                                   // 10 queue
    }
}
