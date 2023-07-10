using EShop.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Security.Claims;

namespace EShop.Web.Controllers
{
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext _context;
        public OrdersController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var userOrders = _context.Orders
                .Where(order => order.OwnerId == userId)
                .ToList();

            foreach (var order in userOrders)
            {
                _context.Entry(order)
                    .Collection(o => o.ProductInOrders)
                    .Load();

                foreach (var productInOrder in order.ProductInOrders)
                {
                    _context.Entry(productInOrder)
                        .Reference(pio => pio.Product)
                        .Load();
                }
            }

            var productsInUserOrders = userOrders
                .SelectMany(order => order.ProductInOrders.Select(pio => pio.Product))
                .ToList();

            ViewBag.ProductsInUserOrders = productsInUserOrders;

            return View();
        }
    }
}
