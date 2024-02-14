using Braintree;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using SMS_DataAccess.Data;
using SMS_DataAccess.Repository;
using SMS_DataAccess.Repository.IRepository;
using SMS_Models;
using SMS_Models.ViewModels;
using SMS_Utility;
using SMS_Utility.BrainTreePayment.Interface;
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
        private readonly IOrderHeaderRepository _orderHeaderRepository;
        private readonly IOrderDetailRepository _orderDetailRepository;
        private readonly IBrainTreeGate _brainTree;
        
        [BindProperty]
        public ProductUserVM ProductUserVM { get; set; }

        public CartController(IWebHostEnvironment webHostEnvironment, 
            IEmailSender emailSender,
            IBrainTreeGate brainTree,
            IProductRepository productRepository,
            IApplicationUserRepository applicationUserRepository,
            IInquiryHeaderRepository inquiryHeaderRepository,
            IInquiryDetailRepository inquiryDetailRepository,
            IOrderHeaderRepository orderHeaderRepository,
            IOrderDetailRepository orderDetailRepository
            )
        {
            _webHostEnvironment = webHostEnvironment;
            _emailSender = emailSender;
            _brainTree = brainTree;
            _productRepository = productRepository;
            _applicationUserRepository = applicationUserRepository;
            _inquiryHeaderRepository = inquiryHeaderRepository;
            _inquiryDetailRepository = inquiryDetailRepository;
            _orderHeaderRepository = orderHeaderRepository;
            _orderDetailRepository = orderDetailRepository;
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
            IEnumerable<Product> prodListTemp = _productRepository.GetAll(x => prodInCart.Contains(x.Id));
            IList<Product> prodList = new List<Product>();

            foreach (var obj in shoppingCartList)
            {
                Product prodTemp = prodListTemp.FirstOrDefault(x => x.Id == obj.ProductId);
                prodTemp.TempCount = obj.Count;
                prodList.Add(prodTemp);
            }

            return View(prodList);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Index")]
        public IActionResult IndexPost(IEnumerable<Product> prodList)
        {
            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();

            foreach (var prod in prodList)
            {
                shoppingCartList.Add(new ShoppingCart()
                {
                    ProductId = prod.Id,
                    Count = prod.TempCount
                });
            }

            HttpContext.Session.Set(WebConstant.SessionCart, shoppingCartList);

            return RedirectToAction(nameof(Summary));
        }

        public IActionResult Summary()
        {
            ApplicationUser applicationUser;
            
            if (User.IsInRole(WebConstant.AdminRole))
            {
                if(HttpContext.Session.Get<int>(WebConstant.SessionInquiryId) != 0)
                {
                    InquiryHeader inquiryHeader = _inquiryHeaderRepository.FirstOrDefault(x => x.Id == (HttpContext.Session.Get<int>(WebConstant.SessionInquiryId)));
                    applicationUser = new ApplicationUser()
                    {
                        Email = inquiryHeader.Email,
                        FullName = inquiryHeader.FullName,
                        PhoneNumber = inquiryHeader.PhoneNumber
                    };
                }
                else
                {
                    applicationUser = new ApplicationUser();
                }

                var gateway = _brainTree.GetGateway();
                var clientToken = gateway.ClientToken.Generate();

                ViewBag.ClientToken = clientToken;
            }
            else
            {
                var claimIden = (ClaimsIdentity)User.Identity;
                var claim = claimIden.FindFirst(ClaimTypes.NameIdentifier);
                //var userId = User.FindFirstValue(ClaimTypes.Name);

                applicationUser = _applicationUserRepository.FirstOrDefault(x => x.Id == claim.Value);
            }

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
                ApplicationUser = applicationUser,
            }; 

            foreach (var obj in shoppingCartList)
            {
                Product prodTemp = _productRepository.FirstOrDefault(x => x.Id == obj.ProductId);
                prodTemp.TempCount = obj.Count;
                ProductUserVM.ProductList.Add(prodTemp);
            }

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

        public IActionResult ClearCart(int id)
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Summary")]
        public async Task<IActionResult> SummaryPost(IFormCollection collection, ProductUserVM productUserVM)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            if (User.IsInRole(WebConstant.AdminRole))
            {
                var orderHeader = new OrderHeader()
                {
                    CreatedByUserId = claim.Value,
                    FinalOrderTotal = ProductUserVM.ProductList.Sum(x => x.TempCount * x.Price),
                    City = ProductUserVM.ApplicationUser.City,
                    StreetAddress = ProductUserVM.ApplicationUser.StreetAdress,
                    State = ProductUserVM.ApplicationUser.State,
                    PostalCode = ProductUserVM.ApplicationUser.PostalCode,
                    FullName = ProductUserVM.ApplicationUser.FullName,
                    Email = ProductUserVM.ApplicationUser.Email,
                    PhoneNumber = ProductUserVM.ApplicationUser.PhoneNumber,
                    OrderDate = DateTime.Now,
                    OrderStatus = WebConstant.StatusPending
                };

                _orderHeaderRepository.Add(orderHeader);
                _orderHeaderRepository.Save();

                foreach (var product in productUserVM.ProductList)
                {
                    var orderDetail = new OrderDetail()
                    {
                        OrderHeaderId = orderHeader.Id,
                        PricePerOne = product.Price,
                        Count = product.TempCount,
                        ProductId = product.Id
                    };
                    _orderDetailRepository.Add(orderDetail);
                }

                _orderDetailRepository.Save();

                string nonceFromTheClient = collection["payment_method_nonce"];

                var request = new TransactionRequest
                {
                    Amount = Convert.ToDecimal(orderHeader.FinalOrderTotal),
                    PaymentMethodNonce = nonceFromTheClient,
                    OrderId = orderHeader.Id.ToString(),
                    Options = new TransactionOptionsRequest
                    {
                        SubmitForSettlement = true
                    }
                };

                var gateway = _brainTree.GetGateway();
                Result<Transaction> result = gateway.Transaction.Sale(request);

                if (result.Target.ProcessorResponseText == "Approved")
                {
                    orderHeader.TransactionId = result.Target.Id;
                    orderHeader.OrderStatus = WebConstant.StatusApproved;
                }
                else
                {
                    orderHeader.OrderStatus = WebConstant.StatusCancelled;
                }
                _orderHeaderRepository.Save();

                return RedirectToAction(nameof(InquiryConfirmation), new { id = orderHeader.Id });
            }
            else
            {
                var PathToTemplate = _webHostEnvironment.WebRootPath + Path.DirectorySeparatorChar.ToString() + "Templates" + Path.DirectorySeparatorChar.ToString() + "Inquiry.html";
                var subject = "New Inquiry";
                var htmlBody = "";

                using (var sr = new StreamReader(PathToTemplate))
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
            }

            return RedirectToAction(nameof(InquiryConfirmation));
        }

        public IActionResult InquiryConfirmation(int id = 0)
        {
            var orderHeader = _orderHeaderRepository.FirstOrDefault(x => x.Id == id);
            HttpContext.Session.Clear();
            return View(orderHeader);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateCart(IEnumerable<Product> prodList)
        {
            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();

            foreach (var prod in prodList)
            {
                shoppingCartList.Add(new ShoppingCart()
                {
                    ProductId = prod.Id,
                    Count = prod.TempCount
                });
            }

            HttpContext.Session.Set(WebConstant.SessionCart, shoppingCartList);

            return RedirectToAction(nameof(Index));
        }
    }
}
