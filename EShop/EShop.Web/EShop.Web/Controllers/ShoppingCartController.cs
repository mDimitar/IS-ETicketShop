using EShop.Web.Data;
using EShop.Web.Models.DomainModels;
using EShop.Web.Models.DTO;
using EShop.Web.Models.Relationship;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EShop.Web.Controllers
{
    [Authorize]
    public class ShoppingCartController : Controller
    {

        private readonly ApplicationDbContext _context;

        public ShoppingCartController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var loggedInUser = await _context.Users.Where(z => z.Id == userId)
                .Include(z => z.UserShoppingCart)
                .Include("UserShoppingCart.ProductInShoppingCarts")
                .Include("UserShoppingCart.ProductInShoppingCarts.Product")
                .FirstOrDefaultAsync();

            var UserCart = loggedInUser.UserShoppingCart;

            var allProducts = UserCart.ProductInShoppingCarts.ToList();

            var allProductPrices = allProducts.Select(z => new
            {
                ProductPrice = z.Product.ProductPrice,
                Quantity = z.Quantity
            }).ToList();

            double totalPrice = 0.0;

            foreach (var item in allProductPrices)
            {
                totalPrice += item.Quantity * item.ProductPrice;
            }

            ShoppingCartDto model = new ShoppingCartDto
            {
                Products = allProducts,
                TotalPrice = totalPrice
            };


            return View(model);
        }

        public async Task<IActionResult> Delete(Guid? id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!string.IsNullOrEmpty(userId))
            {
                var loggedInUser = await _context.Users.Where(z => z.Id == userId)
               .Include(z => z.UserShoppingCart)
               .Include("UserShoppingCart.ProductInShoppingCarts")
               .Include("UserShoppingCart.ProductInShoppingCarts.Product")
               .FirstOrDefaultAsync();

                var UserCart = loggedInUser.UserShoppingCart;

                var itemToDelete = UserCart.ProductInShoppingCarts.Where(z => z.ProductId.Equals(id)).FirstOrDefault();

                UserCart.ProductInShoppingCarts.Remove(itemToDelete);

                _context.Update(UserCart);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index", "ShoppingCart");
        }

        public async Task<IActionResult> OrderNow()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!string.IsNullOrEmpty(userId))
            {
                var loggedInUser = await _context.Users.Where(z => z.Id == userId)
               .Include(z => z.UserShoppingCart)
               .Include("UserShoppingCart.ProductInShoppingCarts")
               .Include("UserShoppingCart.ProductInShoppingCarts.Product")
               .FirstOrDefaultAsync();

                var UserCart = loggedInUser.UserShoppingCart;

                Order userOrder = new Order
                {
                    Id = Guid.NewGuid(),
                    OwnerId = loggedInUser.Id,
                    Owner = loggedInUser
                };

                _context.Add(userOrder);
                await _context.SaveChangesAsync();

                var productsInOrder = UserCart.ProductInShoppingCarts.Select(z => new ProductInOrder
                {
                    ProductId = z.Product.Id,
                    Product = z.Product,
                    OrderId = userOrder.Id,
                    UserOrder = userOrder
                }).ToList();

                foreach (var item in productsInOrder)
                {
                    _context.Add(item);
                }

                await _context.SaveChangesAsync();

                UserCart.ProductInShoppingCarts.Clear();
                _context.Update(UserCart);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index", "Products");
        }
    }
}
