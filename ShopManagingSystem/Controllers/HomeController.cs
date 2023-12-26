using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopManagingSystem.Data;
using ShopManagingSystem.Models;
using ShopManagingSystem.Models.ViewModels;
using ShopManagingSystem.ServiceExtensions;
using System.Diagnostics;

namespace ShopManagingSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _database;

        public HomeController(ILogger<HomeController> logger, AppDbContext database)
        {
            _logger = logger;
            _database = database;
        }

        public IActionResult Index()
        {
            HomeVm homeVm = new HomeVm()
            {
                Products = _database.Products.Include(x => x.Category).Include(x => x.ApplicationType),
                Categories = _database.Categories
            };

            return View(homeVm);
        }

        public IActionResult Details(int id)
        {
            var shoppingCartList = new List<ShoppingCart>();

            if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WebConstant.SessionCart) != null &&
                HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WebConstant.SessionCart).Count() > 0)
            {
                shoppingCartList = HttpContext.Session.Get<List<ShoppingCart>>(WebConstant.SessionCart);
            }

            var detailsVM = new DetailsVM()
            {
                Product = _database.Products.Include(x => x.Category).Include(x => x.ApplicationType).FirstOrDefault(x => x.Id == id),
                ExistsInCart = shoppingCartList.Select(x => x.ProductId).Contains(id)
            };
            
            return View(detailsVM);
        }

        [HttpPost, ActionName("Details")]
        public IActionResult DetailsPost(int id)
        {
            var shoppingCartList = new List<ShoppingCart>();

            if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WebConstant.SessionCart) != null &&
                HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WebConstant.SessionCart).Count() > 0)
            {
                shoppingCartList = HttpContext.Session.Get<List<ShoppingCart>>(WebConstant.SessionCart);
            }

            shoppingCartList.Add(new ShoppingCart()
            {
                ProductId = id
            });

            HttpContext.Session.Set(WebConstant.SessionCart, shoppingCartList);

            return RedirectToAction(nameof(Index));
        }

        public IActionResult RemoveFromCart(int id)
        {
            var shoppingCartList = new List<ShoppingCart>();

            if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WebConstant.SessionCart) != null &&
                HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WebConstant.SessionCart).Count() > 0)
            {
                shoppingCartList = HttpContext.Session.Get<List<ShoppingCart>>(WebConstant.SessionCart);
            }


            var itemToRemove = shoppingCartList.SingleOrDefault(x => x.ProductId == id);

            if (itemToRemove != null) 
            {
                shoppingCartList.Remove(itemToRemove);
            }

            HttpContext.Session.Set(WebConstant.SessionCart, shoppingCartList);

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}