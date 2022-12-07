using Ghumfir.Areas.Identity.Data;
using Ghumfir.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace Ghumfir.Controllers
{
    public class HomeController : Controller
    {
        private readonly ghumfirContext _context;
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<User> _userManager;

        public HomeController(ILogger<HomeController> logger, ghumfirContext context, UserManager<User> userManager)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
        public async Task<IActionResult> Packages()
        {
            return View(await _context.Packages.ToListAsync());
        }
        public IActionResult Bookings(int id, string? returnUrl = null)
        {
            string userid = _userManager.GetUserId(User);
            returnUrl ??= Url.Content("~/Identity/Account/Login");
            if(userid == null)
            {
                return LocalRedirect(returnUrl);
            }
            var package = _context.Packages.Where(x => x.Id == id);
            HttpContext.Session.SetInt32("id", (int)id);
            return View(package);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Thankyou(string first_name, string last_name, string email, string phone)
        {
            var packageid = Convert.ToInt32(HttpContext.Session.GetInt32("id"));
            string userid = _userManager.GetUserId(User);
            var booking = _context.Bookings.Where(x => x.UserId == userid).Where(x => x.Status == "Pending").Include(x => x.Package);
            Booking bookings = new Booking();
            bookings.UserId = userid;
            bookings.FirstName = first_name;
            bookings.LastName = last_name;
            bookings.Email = email;
            bookings.Phone = phone;
            bookings.PackageId = packageid;
            bookings.Status = "Pending";
            _context.Add(bookings);
            await _context.SaveChangesAsync();
            return View(booking);
        }
        public ActionResult PayNow()
        {
            return Redirect("https://www.sandbox.paypal.com/cgi-bin/webscr?cmd=_xclick&business=ghumfir@business.example.com&item_name=ghumfir&return=https://localhost:7059/Home/Index&amount=10.00");
        }
        public ActionResult Contacts()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Contacts(string name,string email,string subject,string message )
        {
            Contact contact = new Contact();
            contact.Name = name;
            contact.Email = email;
            contact.Subject = subject;
            contact.Message = message;
            _context.Add(contact);
            await _context.SaveChangesAsync();
            TempData["Message"] = "Your message has been sent! We will try to reach you soon.";
            return View();
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}