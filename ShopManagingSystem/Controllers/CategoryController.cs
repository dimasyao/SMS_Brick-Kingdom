using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMS_DataAccess.Data;
using SMS_Models;
using SMS_Utility;
using System.Data;

namespace ShopManagingSystem.Controllers
{
    [Authorize(Roles = WebConstant.AdminRole)]
    public class CategoryController : Controller
    {
        private readonly AppDbContext _database;

        public CategoryController(AppDbContext database)
        {
            _database = database;
        }

        public IActionResult Index()
        {
            IEnumerable<Category> categories = _database.Categories;
            return View(categories);
        }

        //Get
        public IActionResult Create()
        {
            return View();
        }

        //Post
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Category category)
        {
            if (ModelState.IsValid)
            {
                _database.Categories.Add(category);
                _database.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(category);   
        }

        //Get
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            var category = _database.Categories.Find(id);

            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        //Post
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Category category)
        {
            if (ModelState.IsValid)
            {
                _database.Categories.Update(category);
                _database.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(category);
        }

        //Get
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            var category = _database.Categories.Find(id);

            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        //Post
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int? id)
        {
            var category = _database.Categories.Find(id);

            if (category == null)
                return NotFound();

            _database.Categories.Remove(category);
            _database.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}
