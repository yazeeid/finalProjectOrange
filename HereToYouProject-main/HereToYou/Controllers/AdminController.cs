using HereToYou.Context;
using HereToYou.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HereToYou.Controllers
{
    public class AdminController : Controller
    {
      
        
            private readonly MyContext _context;
            public AdminController(MyContext myContext)
            {
                _context = myContext;
            }
        public async Task<IActionResult> Index()
        {
            // Check if the user is authenticated and has the required role
            if (HttpContext.Session.GetInt32("userId") != null && HttpContext.Session.GetInt32("RoleId") == 1)
            {
                // Fetch data from the database
                var totalSale = _context.Orders.Sum(p => p.TotalAmount);
                var productsCount = _context.Products.Count();
                var userCount = _context.Users.Count();
                var yesterday = DateTime.Today.AddDays(-1).Date;
                var today = DateTime.Today.Date.AddDays(1).AddTicks(-1);

                var lastDaySales = _context.Orders
                    .Where(p => p.CreatedAt >= yesterday && p.CreatedAt <= today)
                    .Sum(p => (decimal?)p.TotalAmount) ?? 0;

                // Fetch contact messages
                var contactMessages = await _context.ContactUsMessages
                    .Select(c => new ContactUs
                    {
                        ContactUsId = c.ContactUsId,
                        Name = c.Name,
                        Email = c.Email,
                        Subject = c.Subject,
                        Message = c.Message,
                        DateSubmitted = c.DateSubmitted,
                    })
                    .ToListAsync();

                // Store data in ViewBag or ViewData
                ViewBag.Name = HttpContext.Session.GetString("name");
                //ViewBag.Image = HttpContext.Session.GetString("image");
                ViewBag.TotalSale = totalSale;
                ViewBag.ProductsCount = productsCount;
                ViewBag.UserCount = userCount;
                ViewBag.LastDaySales = lastDaySales;

                // Pass the contact messages to the view using ViewData
                ViewData["ContactMessages"] = contactMessages;

                return View();
            }
            else
            {
                return RedirectToAction("Login", "Authentication");
            }
        }

        public async Task<IActionResult> Product()
        {
            if (HttpContext.Session.GetInt32("userId") != null && HttpContext.Session.GetInt32("RoleId") == 1)
            {
                var myContext = _context.Products.Include(p => p.Category);
                return View(await myContext.ToListAsync());
            }
            else
            {
                return RedirectToAction("Login", "authentication");


            }
            return View();
        }
        public async Task<IActionResult> Category()
        {
            if (HttpContext.Session.GetInt32("userId") != null && HttpContext.Session.GetInt32("RoleId") == 1)
            {
                var myContext = _context.Categories;
                return View(await myContext.ToListAsync());
            }
            else
            {
                return RedirectToAction("Login", "authentication");
            }
            return View();
        }

        public async Task<IActionResult> Order()
        {
            if (HttpContext.Session.GetInt32("userId") != null && HttpContext.Session.GetInt32("RoleId") == 1)
            {
                var myContext = _context.OrderItems
                                    .Include(oi => oi.Order)         // Include the Order entity related to OrderItem
                                        .ThenInclude(o => o.User)   // Include the User entity related to Order
                                    .Include(oi => oi.Product)       // Include the Product entity related to OrderItem
                                    .ToListAsync();                  // Execute the query

                var orderItems = await myContext;

                return View(orderItems);
            }
            else
            {
                return RedirectToAction("Login", "authentication");
            }
            return View();
        }


    }
    }


