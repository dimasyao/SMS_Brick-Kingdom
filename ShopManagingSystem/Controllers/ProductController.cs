using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShopManagingSystem.Data;
using ShopManagingSystem.Models;
using ShopManagingSystem.Models.ViewModels;

namespace ShopManagingSystem.Controllers
{
    [Authorize(Roles = WebConstant.AdminRole)]
    public class ProductController : Controller
    {
        private readonly AppDbContext _database;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductController(AppDbContext database, IWebHostEnvironment webHostEnvironment) 
        {
            _database = database;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            IEnumerable<Product> products = _database.Products.Include(x => x.Category).Include(x => x.ApplicationType);

            return View(products);
        }

        //Get
        public IActionResult Upsert(int? id)
        {
            ProductVM productVM = new ProductVM()
            {
                Product = new Product(),
                CategorySelectList = _database.Categories.Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                }),
                ApplicationTypeSelectList = _database.ApplicationTypes.Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                })
            };

            if (id == null)
            {
                return View(productVM);
            }
            else
            {
                productVM.Product = _database.Products.Find(id);
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
                productVM.CategorySelectList = _database.Categories.Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                });
                productVM.ApplicationTypeSelectList = _database.ApplicationTypes.Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                });


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

                    _database.Products.Add(productVM.Product);
                }
                else
                {
                    var productFromDb = _database.Products.AsNoTracking().FirstOrDefault(x => x.Id == productVM.Product.Id);

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

                    _database.Products.Update(productVM.Product);
                }

                _database.SaveChanges();
                return RedirectToAction("Index");
            }

            productVM.CategorySelectList = _database.Categories.Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.Id.ToString()
            });
            productVM.ApplicationTypeSelectList = _database.ApplicationTypes.Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.Id.ToString()
            });

            return View(productVM);
        }

        //Get - delete
        public IActionResult Delete(int? id)
        {
            if (id == 0 || id == null)
            {
                return NotFound();
            }

            var product = _database.Products.Include(x => x.Category).Include(x => x.ApplicationType).FirstOrDefault(x => x.Id == id);

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
            var product = _database.Products.Find(id);

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

            _database.Products.Remove(product);
            _database.SaveChanges();
            return RedirectToAction("Index");
        }

    }
}
