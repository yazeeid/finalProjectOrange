using ecommerce.Cart;
using HereToYou.Context;
using HereToYou.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace ecommerce.Controllers
{
    public class CartController : Controller
    {
        private readonly ICartService _cartService;
        private readonly IProductService _productService; // Assuming you have a product service to fetch product details
        private readonly MyContext context;
        public CartController(ICartService cartService, IProductService productService, MyContext context)
        {

            this.context = context;
            _cartService = cartService;
            _productService = productService;
        }

        public IActionResult Index()
        {
            var cart = _cartService.GetCart();
            var totalAmount = cart.Sum(item => item.Price * item.Quantity);

            var countOfItem = cart.Count();
            ViewBag.Count = countOfItem;

            HttpContext.Session.SetInt32("countOfItem", countOfItem);
            HttpContext.Session.SetString("totalAmount", totalAmount.ToString());
            ViewBag.totalAmount = totalAmount;

            if (HttpContext.Session.GetInt32("userId") != null)
            {
                ViewBag.Login = "Login";
            }

            return View(cart);
        }



        [HttpGet]
        public IActionResult Checkout()
        {
            int id = Convert.ToInt32(HttpContext.Session.GetInt32("userId"));

            if (id == null)
            {
                RedirectToAction("Login", "Authentication");
            }
            else
            {
                ViewBag.Login = "Login";

            }
            return View("Index","Cart");
        }
        [HttpPost]
        public JsonResult AddToCart(int productId, int quantity)
        {
            var product = _productService.GetProductById(productId);
            if (product != null)
            {
                var cart = _cartService.GetCart();
                _cartService.AddToCart(product, quantity);

                var countOfItem = cart.Count() + 1;
                HttpContext.Session.SetInt32("countOfItem", countOfItem);
                return Json(new { success = true, countOfItem = countOfItem });
            }
            return Json(new { success = false });
        }



        [HttpPost]
        public IActionResult RemoveFromCart(int productId)
        {
            _cartService.RemoveFromCart(productId);

            var cart = _cartService.GetCart();
            var countOfItem = cart.Count();
            HttpContext.Session.SetInt32("countOfItem", countOfItem);

            ViewBag.Count = countOfItem;
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult ClearCart()
        {

            _cartService.ClearCart();
            var cart = _cartService.GetCart();
            var countOfItem = cart.Count();
            HttpContext.Session.SetInt32("countOfItem", countOfItem);
            ViewBag.Count = countOfItem;
            return RedirectToAction("Index");
        }



        [HttpPost]
        public IActionResult IncreaseQuantity(int productId)
        {
            var product = _productService.GetProductById(productId);
            if (product != null)
            {
                _cartService.AddToCart(product, 1);
                var cart = _cartService.GetCart();
                var countOfItem = cart.Count();
                HttpContext.Session.SetInt32("countOfItem", countOfItem);
                ViewBag.Count = countOfItem;

            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult DecreaseQuantity(int productId)
        {
            var cart = _cartService.GetCart();
            var cartItem = cart.FirstOrDefault(item => item.ProductId == productId);
            if (cartItem != null && cartItem.Quantity > 1)
            {
                cartItem.Quantity--;
                _cartService.SaveCart(cart);

                var countOfItem = cart.Count();
                HttpContext.Session.SetInt32("countOfItem", countOfItem);
                ViewBag.Count = countOfItem;
            }
            else
            {
                _cartService.RemoveFromCart(productId);
            }
            return RedirectToAction("Index");
        }


        private bool BankExists(int id)
        {
            return context.Banks.Any(e => e.Id == id);
        }
        public IActionResult ProcessCheckout()
        {
            ViewBag.totalAmount = HttpContext.Session.GetString("totalAmount");

            if (HttpContext.Session.GetInt32("userId") == null)
            {
                return RedirectToAction("Login", "authentication");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProcessCheckout(Bank bank)
        {
            if (bank == null)
            {
                return BadRequest("Invalid bank details.");
            }


            // Validate the bank details
            var accountInBank = await context.Banks
                .Where(b => b.CVV == bank.CVV
                            && b.CardHolder == bank.CardHolder
                            && b.CardNumber == bank.CardNumber)
                .FirstOrDefaultAsync();

            if (accountInBank != null)
            {
                ViewBag.totalAmount = HttpContext.Session.GetString("totalAmount");
                // Calculate the total amount of the cart
                var cartItems = _cartService.GetCart();
                var totalAmount = cartItems.Sum(item => item.Price * item.Quantity);

                if (accountInBank.Balance >= totalAmount)
                {
                    try
                    {
                        // Deduct the amount from the bank balance
                        accountInBank.Balance -= totalAmount;
                        context.Update(accountInBank);

                        // Create the order
                        var order = new Order
                        {
                            UserId = HttpContext.Session.GetInt32("userId"),
                            TotalAmount = totalAmount,
                            Status = "Pending",
                            OrderItems = cartItems.Select(cartItem => new OrderItem
                            {
                                ProductId = cartItem.ProductId,
                                Quantity = cartItem.Quantity,
                                Price = cartItem.Price
                            }).ToList()
                        };

                        context.Orders.Add(order);
                        await context.SaveChangesAsync();

                        // Clear the cart
                        _cartService.ClearCart();

                        return RedirectToAction("SuccessfulPayment");
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!BankExists(accountInBank.Id))
                        {
                            return NotFound();
                        }
                        else
                        {
                            throw;
                        }
                    }
                }
                else
                {
                    ViewBag.Error = "Insufficient balance.";
                    return View();
                }
            }
            else
            {
                ViewBag.Error = "Visa card information is incorrect.";
                return View();
            }

            return View("Checkout", _cartService.GetCart());
        }


        public IActionResult SuccessfulPayment()
        {
            ViewBag.totalAmount = HttpContext.Session.GetString("totalAmount");

            if (HttpContext.Session.GetInt32("userId") != null)
            {
                ViewBag.Login = "Login";
            }
            return View();
        }




    }
}
