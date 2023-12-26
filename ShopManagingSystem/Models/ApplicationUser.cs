using Microsoft.AspNetCore.Identity;

namespace ShopManagingSystem.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }
    }
}
