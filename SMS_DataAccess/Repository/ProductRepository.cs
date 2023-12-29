using Microsoft.AspNetCore.Mvc.Rendering;
using SMS_DataAccess.Data;
using SMS_DataAccess.Repository.IRepository;
using SMS_Models;
using SMS_Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SMS_DataAccess.Repository
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        private readonly AppDbContext _database;

        public ProductRepository(AppDbContext database) : base(database) 
        {
            _database = database;
        }

        public IEnumerable<SelectListItem> GetAllDropDownList(string obj)
        {
            if (obj == WebConstant.CategoryName)
            {
                return _database.Categories.Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                });
            }

            if (obj == WebConstant.ApplicationTypeName)
            {
                return _database.ApplicationTypes.Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                });
            }

            return null;
        }

        public void Update(Product product)
        {
            _database.Products.Update(product);
        }
    }
}
