using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMS_DataAccess.Data;
using SMS_DataAccess.Repository.IRepository;
using SMS_Models;
using SMS_Utility;
using System.Data;

namespace ShopManagingSystem.Controllers
{
    [Authorize(Roles = WebConstant.AdminRole)]
    public class ApplicationTypeController : Controller
    {
        private readonly IApplicationTypeRepository _applicationTypeRepository;

        public ApplicationTypeController(IApplicationTypeRepository applicationTypeRepository) 
        {
            _applicationTypeRepository = applicationTypeRepository;
        }

        public IActionResult Index()
        {
            IEnumerable<ApplicationType> applicationTypes = _applicationTypeRepository.GetAll();
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
            _applicationTypeRepository.Add(applicationType);
            _applicationTypeRepository.Save();
            return RedirectToAction("Index");
        }

        //Get
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            var category = _applicationTypeRepository.Find(id.GetValueOrDefault());

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
                _applicationTypeRepository.Update(applicationType);
                _applicationTypeRepository.Save();
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

            var category = _applicationTypeRepository.Find(id.GetValueOrDefault());

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
            var category = _applicationTypeRepository.Find(id.GetValueOrDefault());

            if (category == null)
                return NotFound();

            _applicationTypeRepository.Remove(category);
            _applicationTypeRepository.Save();
            return RedirectToAction("Index");
        }

    }
}

