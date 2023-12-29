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
    public class ApplicationTypeRepository : Repository<ApplicationType>, IApplicationTypeRepository
    {
        private readonly AppDbContext _database;

        public ApplicationTypeRepository(AppDbContext database) : base(database) 
        {
            _database = database;
        }

        public void Update(ApplicationType applicationType)
        {
            var applicationTypeFromDatabase = base.FirstOrDefault(x => x.Id == applicationType.Id);

            if (applicationType != null) 
            {
                applicationTypeFromDatabase.Name = applicationType.Name;
            }
        }
    }
}
