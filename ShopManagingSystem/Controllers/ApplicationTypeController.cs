using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopManagingSystem.Data;
using ShopManagingSystem.Models;
using System.Data;

namespace ShopManagingSystem.Controllers
{
    [Authorize(Roles = WebConstant.AdminRole)]
    public class ApplicationTypeController : Controller
    {
        private readonly AppDbContext _database;

        public ApplicationTypeController(AppDbContext database) 
        {
            _database = database;
        }

        public IActionResult Index()
        {
            IEnumerable<ApplicationType> applicationTypes = _database.ApplicationTypes;
            return View(applicationTypes);
        }

        //Get
        public IActionResult Create()
        {
            return View();
        }

        //Post
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ApplicationType applicationType)
        {
            _database.ApplicationTypes.Add(applicationType);
            _database.SaveChanges();
            return RedirectToAction("Index");
        }

        //Get
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            var category = _database.ApplicationTypes.Find(id);

            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        //Post
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(ApplicationType applicationType)
        {
            if (ModelState.IsValid)
            {
                _database.ApplicationTypes.Update(applicationType);
                _database.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(applicationType);
        }

        //Get
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            var category = _database.ApplicationTypes.Find(id);

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
            var category = _database.ApplicationTypes.Find(id);

            if (category == null)
                return NotFound();

            _database.ApplicationTypes.Remove(category);
            _database.SaveChanges();
            return RedirectToAction("Index");
        }

    }
}

