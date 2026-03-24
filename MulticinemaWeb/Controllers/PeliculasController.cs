using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MulticinemaWeb.Models;

namespace MulticinemaWeb.Controllers
{
    public class PeliculasController : Controller
    {
        private readonly MulticinemaDbContext _context;

        public PeliculasController(MulticinemaDbContext context)
        {
            _context = context;
        }

        // 1. INDEX: Catálogo
        public async Task<IActionResult> Index()
        {
            var peliculas = await _context.Peliculas.ToListAsync();
            return View(peliculas);
        }

        // 2. DETAILS: Detalles y Horarios
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var pelicula = await _context.Peliculas
                .Include(p => p.Funciones)
                .ThenInclude(f => f.IdSalaNavigation)
                .FirstOrDefaultAsync(m => m.IdPelicula == id);

            if (pelicula == null) return NotFound();

            return View(pelicula);
        }

        // (GET): Muestra el mapa
        public async Task<IActionResult> SeleccionAsientos(int? id)
        {
            // NOTA: En la vista Details el link envía "id", así que aquí recibimos "id"
            if (id == null) return NotFound();

            var funcion = await _context.Funciones
                .Include(f => f.IdPeliculaNavigation)
                .Include(f => f.IdSalaNavigation)
                .FirstOrDefaultAsync(m => m.IdFuncion == id);

            if (funcion == null) return NotFound();

            // Buscar asientos ocupados para pintar de rojo
            var asientosOcupados = await _context.Boletos
                .Where(b => b.IdFuncion == id)
                .Select(b => b.AsientoCodigo)
                .ToListAsync();

            ViewBag.AsientosOcupados = asientosOcupados;

            return View(funcion);
        }

        // 4. CONFIRMACIÓN (POST): Recibe la compra y guarda en BD
        // El nombre "Confirmacion" debe coincidir con el asp-action del formulario
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Confirmacion(int IdFuncion, string AsientosSeleccionados)
        {
            // Validación: Si no eligió asientos, lo devolvemos
            if (string.IsNullOrEmpty(AsientosSeleccionados))
            {
                return RedirectToAction("SeleccionAsientos", new { id = IdFuncion });
            }

            var funcion = await _context.Funciones
                .Include(f => f.IdPeliculaNavigation) // Incluimos info para la vista de ticket final
                .Include(f => f.IdSalaNavigation)
                .FirstOrDefaultAsync(f => f.IdFuncion == IdFuncion);

            if (funcion == null) return NotFound();

            // Convertir "A1,A2" en lista
            var listaAsientos = AsientosSeleccionados.Split(',');
            var boletosGuardados = new List<Boleto>(); // Lista para pasar a la vista final

            foreach (var asiento in listaAsientos)
            {
                var nuevoBoleto = new Boleto
                {
                    IdFuncion = IdFuncion,
                    IdUsuario = 1, // TEMPORAL: ID 1 fijo hasta tener Login
                    AsientoCodigo = asiento,
                    PrecioPagado = funcion.Precio,
                    CodigoQr = Guid.NewGuid().ToString(),
                    Estado = "Pagado",
                    FechaCompra = DateTime.Now
                };

                _context.Boletos.Add(nuevoBoleto);
                boletosGuardados.Add(nuevoBoleto);
            }

            await _context.SaveChangesAsync();

            // ViewBag para mostrar los datos en la pantalla final de "Gracias por su compra"
            ViewBag.Pelicula = funcion.IdPeliculaNavigation.Titulo;
            ViewBag.Sala = funcion.IdSalaNavigation.NombreSala;
            ViewBag.Hora = funcion.FechaHoraInicio;
            ViewBag.Total = funcion.Precio * listaAsientos.Length;

            // Retornamos la vista con la lista de boletos generados
            return View(boletosGuardados);
        }
    }
}