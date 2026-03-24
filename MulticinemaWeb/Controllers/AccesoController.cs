using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MulticinemaWeb.Models;
using Microsoft.AspNetCore.Http;
using System; 

namespace MulticinemaWeb.Controllers
{
    public class AccesoController : Controller
    {
        private readonly MulticinemaDbContext _context;

        public AccesoController(MulticinemaDbContext context)
        {
            _context = context;
        }

//parte del login
        public IActionResult Login()
        {
     
            if (HttpContext.Session.GetString("UsuarioNombre") != null)
                return RedirectToAction("Index", "Home");

         
            if (Request.Cookies.ContainsKey("CorreoGuardado"))
            {
                ViewBag.CorreoGuardado = Request.Cookies["CorreoGuardado"];
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string correo, string password, bool guardarSesion)
        {
            // verificacion del correo y password 
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Correo == correo && u.PasswordHash == password);

            if (usuario != null)
            {
                // 1. Crea la Sesión
                CrearSesion(usuario);

           
                if (guardarSesion)
                {
                    CookieOptions options = new CookieOptions();
                    options.Expires = DateTime.Now.AddDays(7);
                    Response.Cookies.Append("CorreoGuardado", correo, options);
                }
                else
                {
                    Response.Cookies.Delete("CorreoGuardado");
                }

               
                // Si es Admin, va a dashboard
                if (usuario.Rol == "Admin")
                {
                    return RedirectToAction("Index", "Home");
                }

                // Si es Cliente, va directo a comprar
                return RedirectToAction("Index", "Peliculas");
            }

            ViewBag.Error = "Credenciales incorrectas";
            return View();
        }

      // seccion de registro
        public IActionResult Registrar()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Registrar(Usuario usuario, string passwordInput)
        {
            // valida el Dui
            if (string.IsNullOrEmpty(usuario.Dui) || usuario.Dui.Length != 10)
            {
                ViewBag.Error = "El DUI es obligatorio y debe tener el formato correcto (00000000-0).";
                return View(usuario);
            }

            // Validacion de psswrd
            if (!EsPasswordSeguro(passwordInput))
            {
                ViewBag.Error = "La contraseña debe tener al menos 6 caracteres, incluir un número y un símbolo especial (ej: @, #, $, %).";
                return View(usuario);
            }

            // Valida si el correo ya existe
            bool existe = await _context.Usuarios.AnyAsync(u => u.Correo == usuario.Correo);
            if (existe)
            {
                ViewBag.Error = "Este correo ya está registrado.";
                return View(usuario);
            }

            // guarda el usuario
            usuario.PasswordHash = passwordInput;
            usuario.EsInvitado = false;
            usuario.FechaRegistro = DateTime.Now;

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            CrearSesion(usuario);
            return RedirectToAction("Index", "Peliculas");
        }

        // Función auxiliar para validar la contraseña
        private bool EsPasswordSeguro(string pw)
        {
            if (string.IsNullOrEmpty(pw) || pw.Length < 6) return false;

            bool tieneNumero = pw.Any(char.IsDigit);
            bool tieneLetra = pw.Any(char.IsLetter);
            bool tieneSimbolo = pw.Any(ch => !char.IsLetterOrDigit(ch));

            return tieneNumero && tieneLetra && tieneSimbolo;
        }

     //invitado
        public IActionResult IngresarComoInvitado()
        {
            HttpContext.Session.SetString("UsuarioRol", "Invitado");
            HttpContext.Session.SetString("UsuarioNombre", "Invitado");

            return RedirectToAction("Index", "Peliculas");
        }

        public IActionResult Salir()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        // Helper para crear la sesión
        private void CrearSesion(Usuario usuario)
        {
            HttpContext.Session.SetInt32("UsuarioId", usuario.IdUsuario);
            HttpContext.Session.SetString("UsuarioNombre", usuario.NombreCompleto);

            //Guarda el rol real de la BD
            // Si es nulo asumira que es Cliente
            HttpContext.Session.SetString("UsuarioRol", usuario.Rol ?? "Cliente");
        }
    }
}
