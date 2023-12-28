using Microsoft.AspNetCore.Identity;

namespace SMS_Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }
    }
}
