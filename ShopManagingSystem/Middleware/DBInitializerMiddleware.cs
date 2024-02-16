using Microsoft.AspNetCore.Identity;
using SMS_DataAccess.Data;
using SMS_DataAccess.Initializer;

namespace ShopManagingSystem.Middleware
{
    public class DBInitializerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IDBInitializer _dBInitializer;

        public DBInitializerMiddleware(RequestDelegate next, IDBInitializer dBInitializer)
        {
            _next = next;
            _dBInitializer = dBInitializer;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var dbContext = context.RequestServices.GetRequiredService<AppDbContext>();
            UserManager<IdentityUser> userManager = context.RequestServices.GetRequiredService<UserManager<IdentityUser>>();
            RoleManager<IdentityRole> roleManager = context.RequestServices.GetRequiredService<RoleManager<IdentityRole>>(); 

            _dBInitializer.Initialize(dbContext, userManager, roleManager);

            await _next(context);
        }
    }
}
