namespace Orange.Web.Utils
{
    public static class Routes
    {
        public static void Configure(IConfiguration configuration)
        {
            CouponAPI = configuration["CouponAPI"] ?? string.Empty;
            AuthAPI = configuration["AuthAPI"] ?? string.Empty;
        }

        public static string CouponAPI { get; private set; } = string.Empty;
        public static string AuthAPI { get; private set; } = string.Empty;
    }
}
