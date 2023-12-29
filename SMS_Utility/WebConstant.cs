namespace SMS_Utility
{
    public static class WebConstant
    {
        public const string ImagePath = @"\Images\Products";

        public const string SessionCart = "ShoppingCartSessions";

        public const string AdminRole = "Admin";

        public const string AdminEmail = "shop.manager0711@gmail.com";

        public const string CustomerRole = "Customer";

        public static string[] Roles { get; } = { "Admin", "Customer" };

        public const string CategoryName = "Category";

        public const string ApplicationTypeName = "ApplicationType";
    }
}
