namespace Orange.Web.Utils
{
    public static class Routes
    {
        public static void Configure(IConfiguration configuration)
        {
            CouponAPI = configuration["CouponAPI"] ?? string.Empty;
        }

        public static string CouponAPI { get; private set; } = string.Empty;
    }
}
