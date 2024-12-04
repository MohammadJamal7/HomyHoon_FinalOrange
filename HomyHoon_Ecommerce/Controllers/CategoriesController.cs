using HomyHoon_Ecommerce.Data;

using Microsoft.AspNetCore.Mvc;

namespace HomyHoon_Ecommerce.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CategoriesController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {

            return View();
        }
    }
}
