using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using SMS_DataAccess.Data;
using SMS_DataAccess.Repository;
using SMS_DataAccess.Repository.IRepository;
using SMS_Models;
using SMS_Models.ViewModels;
using SMS_Utility;
using SMS_Utility.ServiceExtensions;
using System.Security.Claims;
using System.Text;

namespace ShopManagingSystem.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly IApplicationUserRepository _applicationUserRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IEmailSender _emailSender;
        private readonly IInquiryHeaderRepository _inquiryHeaderRepository;
        private readonly IInquiryDetailRepository _inquiryDetailRepository;
        
        [BindProperty]
        public ProductUserVM ProductUserVM { get; set; }

        public CartController(IWebHostEnvironment webHostEnvironment, 
            IEmailSender emailSender,
            IProductRepository productRepository,
            IApplicationUserRepository applicationUserRepository,
            IInquiryHeaderRepository inquiryHeaderRepository,
            IInquiryDetailRepository inquiryDetailRepository
            )
        {
            _webHostEnvironment = webHostEnvironment;
            _emailSender = emailSender;
            _productRepository = productRepository;
            _applicationUserRepository = applicationUserRepository;
            _inquiryHeaderRepository = inquiryHeaderRepository;
            _inquiryDetailRepository = inquiryDetailRepository;
        }

        public IActionResult Index()
        {
            var shoppingCartList = new List<ShoppingCart>();

            if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WebConstant.SessionCart) != null
                && HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WebConstant.SessionCart).Any())
            {
                shoppingCartList = HttpContext.Session.Get<List<ShoppingCart>>(WebConstant.SessionCart);
            }

            List<int> prodInCart = shoppingCartList.Select(x => x.ProductId).ToList();

            IEnumerable<Product> prodList = _productRepository.GetAll(x => prodInCart.Contains(x.Id));

            return View(prodList);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Index")]
        public IActionResult IndexPost()
        {
            return RedirectToAction(nameof(Summary));
        }

        public IActionResult Summary()
        {
            var claimIden = (ClaimsIdentity)User.Identity;
            var claim = claimIden.FindFirst(ClaimTypes.NameIdentifier);
            //var userId = User.FindFirstValue(ClaimTypes.Name);

            var shoppingCartList = new List<ShoppingCart>();

            if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WebConstant.SessionCart) != null
                && HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WebConstant.SessionCart).Any())
            {
                shoppingCartList = HttpContext.Session.Get<List<ShoppingCart>>(WebConstant.SessionCart);
            }

            List<int> prodInCart = shoppingCartList.Select(x => x.ProductId).ToList();

            IEnumerable<Product> prodList = _productRepository.GetAll(x => prodInCart.Contains(x.Id));

            ProductUserVM = new ProductUserVM()
            {
                ApplicationUser = _applicationUserRepository.FirstOrDefault(x => x.Id == claim.Value),
                ProductList = prodList.ToList(),
            };

            return View(ProductUserVM);
        }

        public IActionResult Remove(int id)
        {
            var shoppingCartList = new List<ShoppingCart>();

            if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WebConstant.SessionCart) != null
                && HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WebConstant.SessionCart).Any())
            {
                shoppingCartList = HttpContext.Session.Get<List<ShoppingCart>>(WebConstant.SessionCart);
            }

            shoppingCartList.Remove(shoppingCartList.FirstOrDefault(x => x.ProductId == id));
            HttpContext.Session.Set<IEnumerable<ShoppingCart>>(WebConstant.SessionCart, shoppingCartList);

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Summary")]
        public async Task<IActionResult> SummaryPost(ProductUserVM productUserVM)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            var PathToTemplate = _webHostEnvironment.WebRootPath + Path.DirectorySeparatorChar.ToString() + "Templates" + Path.DirectorySeparatorChar.ToString() + "Inquiry.html";
            var subject = "New Inquiry";
            var htmlBody = "";

            using (var sr  = new StreamReader(PathToTemplate)) 
            {
                htmlBody = sr.ReadToEnd();
            }

            var productListSB = new StringBuilder();
            foreach (var product in productUserVM.ProductList)
            {
                productListSB.Append($" - Name: {product.Name} <span style='font-size:14px;'> (ID: {product.Id})</span><br />");
            }

            var messageBody = string.Format(htmlBody,
                productUserVM.ApplicationUser.FullName,
                productUserVM.ApplicationUser.Email,
                productUserVM.ApplicationUser.PhoneNumber,
                productListSB.ToString());

            await _emailSender.SendEmailAsync(WebConstant.AdminEmail, subject, messageBody);

            var inquiryHeader = new InquiryHeader()
            {
                ApplicationUserId = claim.Value,
                FullName = productUserVM.ApplicationUser.FullName,
                Email = productUserVM.ApplicationUser.Email,
                PhoneNumber = productUserVM.ApplicationUser.PhoneNumber,
                InquiryDate = DateTime.Now
            };

            _inquiryHeaderRepository.Add(inquiryHeader);
            _inquiryHeaderRepository.Save();

            foreach (var product in productUserVM.ProductList)
            {
                var inquiryDetail = new InquiryDetail()
                {
                    InquiryHeaderId = inquiryHeader.Id,
                    ProductId = product.Id
                };

                _inquiryDetailRepository.Add(inquiryDetail);
            }
            _inquiryDetailRepository.Save();

            return RedirectToAction(nameof(InquiryConfirmation));
        }

        public IActionResult InquiryConfirmation()
        {
            HttpContext.Session.Clear();
            return View();
        }
    }
}
