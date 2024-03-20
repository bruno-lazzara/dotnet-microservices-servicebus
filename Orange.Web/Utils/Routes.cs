namespace Orange.Web.Utils
{
    public static class Routes
    {
        public static void Configure(IConfiguration configuration)
        {
            ProductAPI = configuration["ProductAPI"] ?? string.Empty;
            CouponAPI = configuration["CouponAPI"] ?? string.Empty;
            AuthAPI = configuration["AuthAPI"] ?? string.Empty;
            CartAPI = configuration["CartAPI"] ?? string.Empty;
        }

        public static string ProductAPI { get; private set; } = string.Empty;
        public static string CouponAPI { get; private set; } = string.Empty;
        public static string AuthAPI { get; private set; } = string.Empty;
        public static string CartAPI { get; private set; } = string.Empty;
    }
}
