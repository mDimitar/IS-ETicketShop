using EShop.Repository;
using EShop.Repository.Implementation;
using EShop.Service.Implementation;
using EShop.Service.Interface;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;

namespace EShop.Web.Controllers
{
    public class PaymentController : Controller
    {
        private readonly IShoppingCartService _shoppingCartService;

        public PaymentController(IShoppingCartService shoppingCartService)
        {
            _shoppingCartService = shoppingCartService;
        }
        public IActionResult Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userShoppingCart = _shoppingCartService.GetProductForShoppingCart(userId);
            var price = userShoppingCart.TotalPrice;

            var userProducts = userShoppingCart.Products.ToList();

            ViewBag.price = price;
            ViewBag.userProducts = userProducts;
           

            return View();
        }
        [HttpPost]
        public IActionResult Processing(string stripeToken, string stripeEmail)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userShoppingCart = _shoppingCartService.GetProductForShoppingCart(userId);
            var amount = userShoppingCart.TotalPrice;

            Dictionary<string, string> Metadata = new Dictionary<string, string>();

            Metadata.Add("UserId", userId);


            var options = new ChargeCreateOptions
            {
                Amount = (int)amount,
                Currency = "USD",
                Description = "Finishing the order for the tickets.",
                Source = stripeToken,
                ReceiptEmail = stripeEmail,
                Metadata = Metadata
            };
            var service = new ChargeService();
            Charge charge;

            try
            {
                charge = service.Create(options);
            }
            catch (StripeException e)
            {
                // Handle Stripe-specific exception (e.g., API error)
                // Render appropriate view for failed payment
                return View("PaymentFailed");
            }
            catch (Exception e)
            {
                // Handle general exception
                // Render appropriate view for failed payment
                return View("PaymentFailed");
            }

            if (charge.Status == "succeeded")
            {

                if (!string.IsNullOrEmpty(userId))
                {
                    _shoppingCartService.CreateOrder(userId);
                }
                return View("PaymentSuccess");
            }
            else
            {
                // Payment failed
                // Render appropriate view for failed payment
                return View("PaymentFailed");
            }

            return View();
        }
        private readonly string WebhookSecret = "whsec_OurSigningSecret";

        //Previous actions

        [HttpPost]
        public IActionResult ChargeChange()
        {
            var json = new StreamReader(HttpContext.Request.Body).ReadToEnd();

            try
            {
                var stripeEvent = EventUtility.ConstructEvent(json,
                    Request.Headers["Stripe-Signature"], WebhookSecret, throwOnApiVersionMismatch: true);
                Charge charge = (Charge)stripeEvent.Data.Object;
                switch (charge.Status)
                {
                    case "succeeded":
                        //This is an example of what to do after a charge is successful



                        break;
                    case "failed":
                        //Code to execute on a failed charge
                        
                        break;
                }
            }
            catch (Exception e)
            {
                return BadRequest();
            }
            return Ok();
        }
    }
}
