using Microsoft.AspNetCore.Mvc;
using Orange.Models.DTO;
using Orange.Web.Services.Interfaces;
using Orange.Web.Utils;
using System.IdentityModel.Tokens.Jwt;

namespace Orange.Web.Controllers
{
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;
        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Detail(int orderId)
        {
            OrderHeaderDTO? order = await _orderService.GetOrderByIdAsync(orderId);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(string status)
        {
            List<OrderHeaderDTO> list = [];
            string? userId = null;

            if (!User.IsInRole(Constants.ROLE_ADMIN))
            {
                userId = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Sub).FirstOrDefault()?.Value;
            }

            list = (await _orderService.GetAllOrdersAsync(userId)).ToList();

            switch (status)
            {
                case "approved":
                    list = list.Where(o => o.Status == Constants.STATUS_APPROVED).ToList();
                    break;
                case "readyforpickup":
                    list = list.Where(o => o.Status == Constants.STATUS_READY).ToList();
                    break;
                case "cancelled":
                    list = list.Where(o => o.Status == Constants.STATUS_CANCELED).ToList();
                    break;
                default:
                    break;
            }

            return Json(new { data = list });
        }

        [HttpPost("OrderReadyForPickup")]
        public async Task<IActionResult> OrderReadyForPickup(int orderId)
        {
            var statusUpdated = await _orderService.UpdateOrderStatusAsync(orderId, Constants.STATUS_READY);
            if (statusUpdated)
            {
                TempData["success"] = "Order status updated successfully";
            }
            else
            {
                TempData["error"] = "Error on updating order status";
            }

            return RedirectToAction(nameof(Detail), new { orderId });
        }

        [HttpPost("CompleteOrder")]
        public async Task<IActionResult> CompleteOrder(int orderId)
        {
            var statusUpdated = await _orderService.UpdateOrderStatusAsync(orderId, Constants.STATUS_COMPLETED);
            if (statusUpdated)
            {
                TempData["success"] = "Order status updated successfully";
            }
            else
            {
                TempData["error"] = "Error on updating order status";
            }

            return RedirectToAction(nameof(Detail), new { orderId });
        }

        [HttpPost("CancelOrder")]
        public async Task<IActionResult> CancelOrder(int orderId)
        {
            var statusUpdated = await _orderService.UpdateOrderStatusAsync(orderId, Constants.STATUS_CANCELED);
            if (statusUpdated)
            {
                TempData["success"] = "Order status updated successfully";
            }
            else
            {
                TempData["error"] = "Error on updating order status";
            }

            return RedirectToAction(nameof(Detail), new { orderId });
        }
    }
}
