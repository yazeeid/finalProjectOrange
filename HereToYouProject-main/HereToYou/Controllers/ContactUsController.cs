
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

using HereToYou.Context;
using HereToYou.Models;

namespace ecommerce.Controllers
{
    public class ContactUsController : Controller
    {
        private readonly MyContext _context;

        public ContactUsController(MyContext context)
        {
            _context = context;
        }

        // GET: ContactUs
        public async Task<IActionResult> Index()
        {
            ViewBag.name = HttpContext.Session.GetString("name");
            var myContext = _context.ContactUsMessages.Include(c => c.User);
            return View(await myContext.ToListAsync());
        }

        // GET: ContactUs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.ContactUsMessages == null)
            {
                return NotFound();
            }

            var contactUs = await _context.ContactUsMessages
                .Include(c => c.User)
                .FirstOrDefaultAsync(m => m.ContactUsId == id);
            if (contactUs == null)
            {
                return NotFound();
            }

            return View(contactUs);
        }


        // GET: ContactUs/Create
        public IActionResult Create()
        {
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: ContactUs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Email,Subject,Message,UserId")] ContactUs contactUs)
        {

            _context.Add(contactUs);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", contactUs.UserId);
            return View(contactUs);
        }

        // GET: ContactUs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.ContactUsMessages == null)
            {
                return NotFound();
            }

            var contactUs = await _context.ContactUsMessages.FindAsync(id);
            if (contactUs == null)
            {
                return NotFound();
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", contactUs.UserId);
            return View(contactUs);
        }

        // POST: ContactUs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ContactUsId,Name,Email,Subject,Message,DateSubmitted,UserId")] ContactUs contactUs)
        {
            if (id != contactUs.UserId)
            {
                return NotFound();
            }


            try
            {
                _context.Update(contactUs);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ContactUsExists((int)contactUs.UserId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }

                return RedirectToAction(nameof(Index));
            }
            ViewData["UserId"] = new SelectList(_context.Users, "ContactUsId", "ContactUsId", contactUs.UserId);
            return View(contactUs);
        }

        // GET: ContactUs/Delete/5
        // GET: ContactUs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.ContactUsMessages == null)
            {
                return NotFound();
            }

            var contactUs = await _context.ContactUsMessages
                .Include(c => c.User)
                .FirstOrDefaultAsync(m => m.ContactUsId == id);
            if (contactUs == null)
            {
                return NotFound();
            }

            return View(contactUs);
        }

        // POST: ContactUs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var contactUs = await _context.ContactUsMessages.FindAsync(id);
            if (contactUs != null)
            {
                _context.ContactUsMessages.Remove(contactUs);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool ContactUsExists(int id)
        {
            return (_context.ContactUsMessages?.Any(e => e.UserId == id)).GetValueOrDefault();
        }
    }
}

