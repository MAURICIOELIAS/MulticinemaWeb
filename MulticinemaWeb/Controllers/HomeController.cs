using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // Importante
using MulticinemaWeb.Models;
using System.Diagnostics;

namespace MulticinemaWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly MulticinemaDbContext _context;

        public HomeController(MulticinemaDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Traemos todas las películas activas
            var peliculas = await _context.Peliculas
                                          .Where(p => p.Estado != "Inactivo")
                                          .ToListAsync();

            return View(peliculas);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}