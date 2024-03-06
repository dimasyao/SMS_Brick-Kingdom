using Braintree;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using SMS_DataAccess.Repository.IRepository;
using SMS_Models;
using SMS_Models.ViewModels;
using SMS_Utility;
using SMS_Utility.BrainTreePayment.Interface;

namespace ShopManagingSystem.Controllers
{
    [Authorize(Roles = WebConstant.AdminRole)]
    public class OrderController : Controller
    {
        private readonly IOrderHeaderRepository _orderHeaderRepository;
        private readonly IOrderDetailRepository _orderDetailRepository;
        private readonly IBrainTreeGate _brainTree;

        [BindProperty]
        public OrderVM OrderVM { get; set; }

        public OrderController(IBrainTreeGate brainTree,
            IOrderHeaderRepository orderHeaderRepository,
            IOrderDetailRepository orderDetailRepository
            )
        {
            _brainTree = brainTree;
            _orderHeaderRepository = orderHeaderRepository;
            _orderDetailRepository = orderDetailRepository;
        }

        public IActionResult Index(string searchName = null, string searchEmail = null, string searchPhone = null, string Status = null)
        {
            var orderListVM = new OrderListVM()
            {
                OrderHeaders = _orderHeaderRepository.GetAll(),
                StatusItems = WebConstant.listStatus.ToList().Select(x => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { Text = x, Value = x })
            };

            if (!string.IsNullOrEmpty(searchName))
            {
                orderListVM.OrderHeaders = orderListVM.OrderHeaders.Where(x => x.FullName.ToLower().Contains(searchName.ToLower()));
            }

            if (!string.IsNullOrEmpty(searchEmail))
            {
                orderListVM.OrderHeaders = orderListVM.OrderHeaders.Where(x => x.Email.ToLower().Contains(searchEmail.ToLower()));
            }

            if (!string.IsNullOrEmpty(searchPhone))
            {
                orderListVM.OrderHeaders = orderListVM.OrderHeaders.Where(x => x.PhoneNumber.ToLower().Contains(searchPhone.ToLower()));
            }

            if (!string.IsNullOrEmpty(Status) && Status != "--Order Status--")
            {
                orderListVM.OrderHeaders = orderListVM.OrderHeaders.Where(x => x.OrderStatus.Contains(Status));
            }

            return View(orderListVM);
        }

        public IActionResult Details(int id)
        {
            OrderVM = new OrderVM()
            {
                OrderHeader = _orderHeaderRepository.FirstOrDefault(x => x.Id == id),
                OrderDetails = _orderDetailRepository.GetAll(x => x.OrderHeaderId == id, includeProperties: "Product")
            };

            return View(OrderVM);
        }

        [HttpPost]
        public IActionResult StartProcessing()
        {
            OrderHeader orderHeader = _orderHeaderRepository.FirstOrDefault(x => x.Id == OrderVM.OrderHeader.Id);
            orderHeader.OrderStatus = WebConstant.StatusInProcess;
            _orderHeaderRepository.Save();
            TempData[WebConstant.Success] = "Order in process!";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult ShipOrder()
        {
            OrderHeader orderHeader = _orderHeaderRepository.FirstOrDefault(x => x.Id == OrderVM.OrderHeader.Id);
            orderHeader.OrderStatus = WebConstant.StatusShipped;
            orderHeader.ShippingDate = DateTime.Now;
            _orderHeaderRepository.Save();
            TempData[WebConstant.Success] = "Order shipped successfully!";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult CancelOrder()
        {
            OrderHeader orderHeader = _orderHeaderRepository.FirstOrDefault(x => x.Id == OrderVM.OrderHeader.Id);

            var gatewayBraintree = _brainTree.GetGateway();
            var transaction = gatewayBraintree.Transaction.Find(orderHeader.TransactionId);

            if (transaction.Status == TransactionStatus.AUTHORIZED || transaction.Status == TransactionStatus.SUBMITTED_FOR_SETTLEMENT)
            {
                //no refund
                Result<Transaction> resultVoid = gatewayBraintree.Transaction.Void(orderHeader.TransactionId);
            }
            else
            {
                //refund
                Result<Transaction> resultRefund = gatewayBraintree.Transaction.Refund(orderHeader.TransactionId);
            }

            orderHeader.OrderStatus = WebConstant.StatusRefunded;
            _orderHeaderRepository.Save();
            TempData[WebConstant.Success] = "Order details cancel successfully!";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult UpdateOrderDetails()
        {
            OrderHeader orderHeaderfromDb = _orderHeaderRepository.FirstOrDefault(x => x.Id == OrderVM.OrderHeader.Id);

            orderHeaderfromDb.FullName = OrderVM.OrderHeader.FullName;
            orderHeaderfromDb.PhoneNumber = OrderVM.OrderHeader.PhoneNumber;
            orderHeaderfromDb.StreetAddress = OrderVM.OrderHeader.StreetAddress;
            orderHeaderfromDb.City = OrderVM.OrderHeader.City;
            orderHeaderfromDb.State = OrderVM.OrderHeader.State;
            orderHeaderfromDb.PostalCode = OrderVM.OrderHeader.PostalCode;

            _orderHeaderRepository.Save();
            TempData[WebConstant.Success] = "Order details updated successfully";

            return RedirectToAction("Details", "Order", new { id = orderHeaderfromDb.Id});
        }
    }
}
