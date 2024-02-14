using System.Collections.ObjectModel;

namespace SMS_Utility
{
    public static class WebConstant
    {
        public const string ImagePath = @"\Images\Products";

        public const string SessionCart = "ShoppingCartSessions";

        public const string SessionInquiryId = "InquirySessions";

        public const string AdminRole = "Admin";

        public const string AdminEmail = "shop.manager0711@gmail.com";

        public const string CustomerRole = "Customer";

        public const string Success = "Success";
        public const string Error = "Error";

        public static string[] Roles { get; } = { "Admin", "Customer" };

        public const string CategoryName = "Category";

        public const string ApplicationTypeName = "ApplicationType";

        #region OrderStatus
        public const string StatusPending = "Pending";
        public const string StatusApproved = "Approved";
        public const string StatusInProcess = "Processing";
        public const string StatusShipped = "Shipped";
        public const string StatusCancelled = "Cancelled";
        public const string StatusRefunded = "Refunded";

        public static readonly IEnumerable<string> listStatus = new ReadOnlyCollection<string>(
            new List<string> { StatusPending, StatusApproved, StatusInProcess, StatusShipped, StatusCancelled, StatusRefunded }); 
        #endregion

    }
}
