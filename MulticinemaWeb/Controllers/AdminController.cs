using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MulticinemaWeb.Models; // Asegúrate de que este coincida con tu proyecto
using Microsoft.AspNetCore.Hosting;
using AspNetCoreGeneratedDocument; // Necesario para subir archivos (IWebHostEnvironment)

namespace MulticinemaWeb.Controllers
{
    public class AdminController : Controller
    {
        private readonly MulticinemaDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment; // Variable para manejar archivos

        // Constructor: Inyectamos el contexto de BD y el entorno de hosting
        public AdminController(MulticinemaDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // ========================================================
        // 1. VISTA PRINCIPAL (MENU)
        // ========================================================
        public IActionResult Index()
        {
            // Aquí podrías validar si el usuario es Admin en el futuro
            return View();
        }

        // ========================================================
        // 2. GESTIÓN DE PELÍCULAS (CRUD)
        // ========================================================

        // GET: Listar Películas
        public IActionResult GestionarPeliculas()
        {
            var peliculas = _context.Peliculas.ToList();
            return View(peliculas);
        }

        // GET: Mostrar formulario para crear nueva película
        public IActionResult CrearPelicula()
        {
            return View();
        }

        // POST: Guardar nueva película (Con imagen)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CrearPelicula(Pelicula pelicula, IFormFile? imagenArchivo)
        {
            // Ignoramos validación de la lista de funciones porque al crear está vacía
            ModelState.Remove("Funciones");

            if (ModelState.IsValid)
            {
                try
                {
                    // Lógica para guardar la imagen si subieron una
                    if (imagenArchivo != null)
                    {
                        // 1. Generar nombre único
                        string nombreArchivo = Guid.NewGuid().ToString() + Path.GetExtension(imagenArchivo.FileName);
                        // 2. Definir ruta: wwwroot/imagenes/peliculas
                        string rutaCarpeta = Path.Combine(_webHostEnvironment.WebRootPath, "imagenes", "peliculas");

                        // 3. Crear carpeta si no existe
                        if (!Directory.Exists(rutaCarpeta)) Directory.CreateDirectory(rutaCarpeta);

                        // 4. Guardar archivo
                        using (var stream = new FileStream(Path.Combine(rutaCarpeta, nombreArchivo), FileMode.Create))
                        {
                            await imagenArchivo.CopyToAsync(stream);
                        }

                        // 5. Guardar la ruta en la base de datos
                        pelicula.PosterUrl = "/imagenes/peliculas/" + nombreArchivo;
                    }

                    _context.Add(pelicula);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(GestionarPeliculas));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Ocurrió un error al guardar: " + ex.InnerException?.Message);
                }
            }
            return View(pelicula);
        }

        // GET: Mostrar formulario de edición
        public async Task<IActionResult> EditarPelicula(int? id)
        {
            if (id == null) return NotFound();

            var pelicula = await _context.Peliculas.FindAsync(id);
            if (pelicula == null) return NotFound();

            return View(pelicula);
        }

        // POST: Guardar cambios de edición (Con imagen)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditarPelicula(int id, Pelicula pelicula, IFormFile? imagenArchivo)
        {
            if (id != pelicula.IdPelicula) return NotFound();

            ModelState.Remove("Funciones"); // Evitar errores de validación no deseados

            if (ModelState.IsValid)
            {
                try
                {
                    // Si subieron una NUEVA imagen, la procesamos
                    if (imagenArchivo != null)
                    {
                        string nombreArchivo = Guid.NewGuid().ToString() + Path.GetExtension(imagenArchivo.FileName);
                        string rutaCarpeta = Path.Combine(_webHostEnvironment.WebRootPath, "imagenes", "peliculas");

                        if (!Directory.Exists(rutaCarpeta)) Directory.CreateDirectory(rutaCarpeta);

                        using (var stream = new FileStream(Path.Combine(rutaCarpeta, nombreArchivo), FileMode.Create))
                        {
                            await imagenArchivo.CopyToAsync(stream);
                        }

                        pelicula.PosterUrl = "/imagenes/peliculas/" + nombreArchivo;
                    }
                  

                    _context.Update(pelicula);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(GestionarPeliculas));
                }
                catch (DbUpdateException ex)
                {
                    // Este bloque captura el error del "CHECK constraint" (Estado incorrecto)
                    ModelState.AddModelError("", "Error de Base de Datos: Verifique que el ESTADO sea 'Estreno', 'Cartelera' o 'Proximamente'. Detalle: " + ex.InnerException?.Message);
                }
            }
            return View(pelicula);
        }

        // POST: Eliminar Película
        public async Task<IActionResult> EliminarPelicula(int id)
        {
            var pelicula = await _context.Peliculas.FindAsync(id);
            if (pelicula != null)
            {
                _context.Peliculas.Remove(pelicula);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(GestionarPeliculas));
        }

   //agregar funciones

        // Cargar formulario y listas
        public IActionResult AgregarFunciones()
        {
         
            ViewBag.Peliculas = _context.Peliculas.ToList();
            // Carga las salas
            ViewBag.Salas = _context.Salas.ToList();
            return View();
        }

        // POST: Guarda la función
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AgregarFunciones(Funcione funcion)
        {
            // Validaciones manuales
            if (funcion.Precio < 0) ModelState.AddModelError("Precio", "El precio no puede ser negativo.");
            if (funcion.FechaHoraInicio < DateTime.Now) ModelState.AddModelError("FechaHoraInicio", "La fecha debe ser futura.");

            // Remueve las validaciones de navegación que estorban
            ModelState.Remove("IdPeliculaNavigation");
            ModelState.Remove("IdSalaNavigation");

            if (ModelState.IsValid)
            {
                _context.Add(funcion);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // recarga las listas para que no salga error en la vista
            ViewBag.Peliculas = _context.Peliculas.ToList();
            ViewBag.Salas = _context.Salas.ToList();

            return View(funcion);
        }

    //reporte ventas
        public IActionResult ReporteVentas()
        {
            var fechaActual = DateTime.Now;

            var ventasDelMes = _context.Boletos
                .Include(b => b.IdUsuarioNavigation)
                .Include(b => b.IdFuncionNavigation)
                .ThenInclude(f => f.IdPeliculaNavigation) // Para ver titulo pelicula
                .Include(b => b.IdFuncionNavigation)
                .ThenInclude(f => f.IdSalaNavigation)     // Para ver nombre sala
                .Where(b => b.FechaCompra.HasValue &&
                            b.FechaCompra.Value.Month == fechaActual.Month &&
                            b.FechaCompra.Value.Year == fechaActual.Year)
                .ToList();

         
            ViewBag.TotalIngresos = ventasDelMes.Sum(b => b.PrecioPagado);
            ViewBag.Mes = fechaActual.ToString("MMMM");

            return View(ventasDelMes);
        }

    //Gestionar funciones
        public IActionResult GestionarFunciones()
        {
            // muestra las funciones incluyendo Pelicula y Sala para mostrar nombres
            var funciones = _context.Funciones
                .Include(f => f.IdPeliculaNavigation)
                .Include(f => f.IdSalaNavigation)
                .OrderByDescending(f => f.FechaHoraInicio) // Las más recientes primero
                .ToList();

            return View(funciones);
        }

        // BORRAR FUNCIÓN
        public async Task<IActionResult> EliminarFuncion(int id)
        {
            var funcion = await _context.Funciones.FindAsync(id);
            if (funcion != null)
            {
                // Verifica si ya hay boletos vendidos para esta función
                var hayBoletos = _context.Boletos.Any(b => b.IdFuncion == id);

                if (hayBoletos)
                {
                    // Opcional: Podrías mandar un mensaje de error diciendo que no se puede borrar
                    // por ahora solo redirigimos sin borrar para proteger los datos.
                    return RedirectToAction(nameof(GestionarFunciones));
                }

                _context.Funciones.Remove(funcion);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(GestionarFunciones));
        }
    }

}