using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SMS_DataAccess.Data;
using SMS_DataAccess.Repository.IRepository;
using SMS_Models;
using SMS_Models.ViewModels;
using SMS_Utility;

namespace ShopManagingSystem.Controllers
{
    [Authorize(Roles = WebConstant.AdminRole)]
    public class ProductController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductController(IProductRepository productRepository, IWebHostEnvironment webHostEnvironment) 
        {
            _productRepository = productRepository;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            IEnumerable<Product> products = _productRepository.GetAll(includeProperties: "Category,ApplicationType");

            return View(products);
        }

        //Get
        public IActionResult Upsert(int? id)
        {
            ProductVM productVM = new ProductVM()
            {
                Product = new Product(),
                CategorySelectList = _productRepository.GetAllDropDownList(WebConstant.CategoryName),
                ApplicationTypeSelectList = _productRepository.GetAllDropDownList(WebConstant.ApplicationTypeName)
            };

            if (id == null)
            {
                return View(productVM);
            }
            else
            {
                productVM.Product = _productRepository.Find(id.GetValueOrDefault());
                if (productVM.Product == null)
                {
                    return NotFound();
                }
                return View(productVM);
            }    
        }

        //Post
        [HttpPost]
        public IActionResult Upsert(ProductVM productVM)
        {
            if (productVM.Product.Name != "")
            {
                productVM.CategorySelectList = _productRepository.GetAllDropDownList(WebConstant.CategoryName);
                productVM.ApplicationTypeSelectList = _productRepository.GetAllDropDownList(WebConstant.ApplicationTypeName);

                var files = HttpContext.Request.Form.Files;
                string webRootPath = _webHostEnvironment.WebRootPath;

                if (productVM.Product.Id == 0 && files.Count > 0)
                {
                    string upload = webRootPath + WebConstant.ImagePath;
                    string fileName = Guid.NewGuid().ToString();
                    string extension = Path.GetExtension(files[0].FileName);

                    using (var fileStream = new FileStream(Path.Combine(upload, fileName + extension), FileMode.Create))
                    {
                        files[0].CopyTo(fileStream);
                    }

                    productVM.Product.Image = fileName + extension;

                    _productRepository.Add(productVM.Product);
                }
                else
                {
                    var productFromDb = _productRepository.FirstOrDefault(x => x.Id == productVM.Product.Id, isTraking: false);

                    if (files.Count > 0)
                    {
                        string upload = webRootPath + WebConstant.ImagePath;
                        string fileName = Guid.NewGuid().ToString();
                        string extension = Path.GetExtension(files[0].FileName);

                        var oldFilePath = Path.Combine(upload, productFromDb.Image);

                        if (System.IO.File.Exists(oldFilePath))
                        {
                            System.IO.File.Delete(oldFilePath);
                        }

                        using (var fileStream = new FileStream(Path.Combine(upload, fileName + extension), FileMode.Create))
                        {
                            files[0].CopyTo(fileStream);
                        }

                        productVM.Product.Image = fileName + extension;
                    }
                    else
                    {
                        if (productFromDb != null)
                        {
                            productVM.Product.Image = productFromDb.Image;
                        }
                        else
                        {
                            return View(productVM);
                        }
                    }

                    _productRepository.Update(productVM.Product);
                }

                _productRepository.Save();
                return RedirectToAction("Index");
            }

            productVM.CategorySelectList = _productRepository.GetAllDropDownList(WebConstant.CategoryName);
            productVM.ApplicationTypeSelectList = _productRepository.GetAllDropDownList(WebConstant.ApplicationTypeName);

            return View(productVM);
        }

        //Get - delete
        public IActionResult Delete(int? id)
        {
            if (id == 0 || id == null)
            {
                return NotFound();
            }

            var product = _productRepository.FirstOrDefault(x => x.Id == id, includeProperties: "Category,ApplicationType");

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        //Post - delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePost(int? id)
        {
            var product = _productRepository.Find(id.GetValueOrDefault());

            if (product == null)
            {
                return NotFound();
            }

            string upload = _webHostEnvironment.WebRootPath + WebConstant.ImagePath;

            var oldFilePath = Path.Combine(upload, product.Image);

            if (System.IO.File.Exists(oldFilePath))
            {
                System.IO.File.Delete(oldFilePath);
            }

            _productRepository.Remove(product);
            _productRepository.Save();
            return RedirectToAction("Index");
        }

    }
}
