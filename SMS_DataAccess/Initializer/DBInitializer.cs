using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SMS_DataAccess.Data;
using SMS_Models;
using SMS_Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS_DataAccess.Initializer
{
    public class DBInitializer : IDBInitializer
    {
        private readonly AppDbContext _database;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public DBInitializer(AppDbContext appDbContext, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _database = appDbContext;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public void Initialize()
        {
            try
            {
                if (_database.Database.GetPendingMigrations().Count() > 0)
                {
                    _database.Database.Migrate();
                }
            }
            catch (Exception ex) 
            { }

            if (!_roleManager.RoleExistsAsync(WebConstant.AdminRole).GetAwaiter().GetResult())
            {
                _roleManager.CreateAsync(new IdentityRole(WebConstant.AdminRole)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(WebConstant.CustomerRole)).GetAwaiter().GetResult();
            }
            else
            {
                return;
            }

            var result = _userManager.CreateAsync(new ApplicationUser
            {
                UserName = "admin@gmail.com",
                Email = "admin@gmail.com",
                EmailConfirmed = true,
                FullName = "MainAdmin",
                PhoneNumber = "0687467499"
            }, "Crjhjcnm_07").GetAwaiter().GetResult();

            var user = _database.ApplicationUsers.FirstOrDefault(x => x.Email == "admin@gmail.com");
            _userManager.AddToRoleAsync(user, WebConstant.AdminRole).GetAwaiter().GetResult();
        }
    }
}
