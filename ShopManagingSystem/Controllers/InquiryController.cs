using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMS_DataAccess.Repository.IRepository;
using SMS_Models;
using SMS_Models.ViewModels;
using SMS_Utility;
using SMS_Utility.ServiceExtensions;

namespace ShopManagingSystem.Controllers
{
    [Authorize(WebConstant.AdminRole)]
    public class InquiryController : Controller
    {
        private readonly IInquiryDetailRepository _inquiryDetailRepository;
        private readonly IInquiryHeaderRepository _inquiryHeaderRepository;

        [BindProperty]
        public InquiryVM InquiryVM { get; set; }

        public InquiryController(IInquiryDetailRepository inquiryDetailRepository, IInquiryHeaderRepository inquiryHeaderRepository)
        {
            _inquiryDetailRepository = inquiryDetailRepository;
            _inquiryHeaderRepository = inquiryHeaderRepository;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Details(int id)
        {
            InquiryVM = new InquiryVM()
            {
                InquiryHeader = _inquiryHeaderRepository.FirstOrDefault(x => x.Id == id),
                InquiryDetails = _inquiryDetailRepository.GetAll(x => x.InquiryHeaderId == id, includeProperties: "Product")
            };

            return View(InquiryVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Details()
        {
            var shoppingCartList = new List<ShoppingCart>();
            InquiryVM.InquiryDetails = _inquiryDetailRepository.GetAll(x => x.InquiryHeaderId == InquiryVM.InquiryHeader.Id);

            foreach (var detail in InquiryVM.InquiryDetails) 
            {
                var shoppingCart = new ShoppingCart()
                {
                    ProductId = detail.ProductId
                };
                shoppingCartList.Add(shoppingCart);
            }

            HttpContext.Session.Clear();
            HttpContext.Session.Set(WebConstant.SessionCart, shoppingCartList);
            HttpContext.Session.Set(WebConstant.SessionInquiryId, InquiryVM.InquiryHeader.Id);

            return RedirectToAction("Index", "Cart");
        }

        [HttpPost]
        public IActionResult Delete()
        {
            var inquiryHeader = _inquiryHeaderRepository.FirstOrDefault(x => x.Id == InquiryVM.InquiryHeader.Id);
            IEnumerable<InquiryDetail> inquiryDetails = _inquiryDetailRepository.GetAll(x => x.InquiryHeaderId == InquiryVM.InquiryHeader.Id);

            _inquiryDetailRepository.Remove(inquiryDetails);
            _inquiryHeaderRepository.Remove(inquiryHeader);
            _inquiryHeaderRepository.Save();

            return RedirectToAction(nameof(Index));
        }

        #region API CALLS
        [HttpGet]
        public IActionResult GetInquiryList() 
        {
            return Json(new { data = _inquiryHeaderRepository.GetAll() });
        }
        #endregion
    }
}
