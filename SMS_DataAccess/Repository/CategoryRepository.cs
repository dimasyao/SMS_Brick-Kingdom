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
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        private readonly AppDbContext _database;

        public CategoryRepository(AppDbContext database) : base(database) 
        {
            _database = database;
        }

        public void Update(Category category)
        {
            var categoryFromDatabase = base.FirstOrDefault(x => x.Id == category.Id);

            if (categoryFromDatabase != null) 
            { 
                categoryFromDatabase.Name = category.Name;
                categoryFromDatabase.DisplayOrder = category.DisplayOrder;
            }
        }
    }
}
