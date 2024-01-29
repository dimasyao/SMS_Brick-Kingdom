using SMS_DataAccess.Data;
using SMS_DataAccess.Repository.IRepository;
using SMS_Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS_DataAccess.Repository
{
    public class ApplicationUserRepository : Repository<ApplicationUser>, IApplicationUserRepository
    {
        private readonly AppDbContext _database;

        public ApplicationUserRepository(AppDbContext database) : base(database) 
        {
            _database = database;
        }
    }
}
