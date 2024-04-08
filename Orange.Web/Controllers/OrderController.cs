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
        public async Task<IActionResult> GetAll()
        {
            List<OrderHeaderDTO> list = [];
            string? userId = null;

            if (!User.IsInRole(Constants.ROLE_ADMIN))
            {
                userId = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Sub).FirstOrDefault()?.Value;
            }

            list = (await _orderService.GetAllOrdersAsync(userId)).ToList();

            return Json(new { data = list });
        }
    }
}
