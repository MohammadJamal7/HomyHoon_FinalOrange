
using HomyHoon.Models;
using HomyHoon_Ecommerce.Data;
using HomyHoon_Ecommerce.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace HomyHoon.Controllers
{
    public class MessageController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public MessageController(ApplicationDbContext context, UserManager<User> usermanager)
        {
            _context = context;
            _userManager = usermanager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> SaveMessageToDb(IndexPageViewModel model)
        {



            var message = new Message
            {

                SenderName = model.Message.SenderName,
                SenderEmail = model.Message.SenderEmail,
                Subject = model.Message.Subject,
                DateSent = DateTime.Now
            };

            _context.Messages.Add(message);
            _context.SaveChanges();

            return RedirectToAction("Index", "Pages");


    
        }
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var message = await _context.Messages.FindAsync(id);
            if (message != null)
            {
                _context.Messages.Remove(message);
                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }


        public async Task<IActionResult> View(int id)
        {
            Message question = await _context.Messages.FirstOrDefaultAsync(q => q.Id == id);
            var item = 3;
            return View(question);
        }
    }
}