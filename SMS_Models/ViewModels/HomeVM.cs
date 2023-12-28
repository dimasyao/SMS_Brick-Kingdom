namespace SMS_Models.ViewModels
{
    public class HomeVm
    {
        public IEnumerable<Product> Products { get; set; }

        public IEnumerable<Category> Categories { get; set; }
    }
}
