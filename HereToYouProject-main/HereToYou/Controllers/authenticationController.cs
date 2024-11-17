
using HereToYou.Models;
using HereToYou.Context;
using HereToYou.Context;
using HereToYou.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Controllers
{
    public class authenticationController : Controller
    {
        private readonly MyContext _context;
        public authenticationController(MyContext myContext)
        {
            _context = myContext;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login([Bind("Id,Username,PasswordHash,Email,,RoleId")] User user)
        {

            var existingUser = _context.Users.Where(u => u.Email == user.Email && u.PasswordHash == user.PasswordHash).
                Include(p => p.Role).FirstOrDefault();

            if (existingUser != null)
            {
                HttpContext.Session.SetInt32("userId", existingUser.Id);
                HttpContext.Session.SetInt32("RoleId", existingUser.Role.Id);
                int x = Convert.ToInt32(HttpContext.Session.GetInt32("userId"));
                switch (existingUser.RoleId)
                {
                    case 1:
                        return RedirectToAction("Index", "Admin");
                    case 2:

                        return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                ViewBag.LoginError = "username or password is incoorect pleace try again .";
            }
            return View();
        }


		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Register([Bind("Id,Username,PasswordHash,Email,RoleId,Location,phoneNumber")] User user)
		{
			if (string.IsNullOrEmpty(user.Username))
			{
				ViewBag.RegisterError = "Enter your username";
				return View("Login", user);
			}
			if (string.IsNullOrEmpty(user.PasswordHash))
			{
				ViewBag.RegisterError = "Enter your password";
				return View("Login", user);
			}
			if (string.IsNullOrEmpty(user.Email))
			{
				ViewBag.RegisterError = "Enter your email";
				return View("Login", user);
			}

			var existingUser = _context.Users.FirstOrDefault(u => u.Email == user.Email);
			if (existingUser != null)
			{
				ViewBag.RegisterError = "Email is already used";
				return View("Login", user);
			}

			user.RoleId = 2;
			_context.Add(user);
			await _context.SaveChangesAsync();

			ViewBag.IsSuccess = true;
			ViewBag.Message = "Registration Successful!";
			return View("Login");
		}


		public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "authentication");
        }


    }
}