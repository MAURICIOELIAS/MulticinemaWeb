using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MulticinemaWeb.Models
{
    public partial class Usuario
    {
        // 1. Constructor: Inicializamos la lista de boletos vacía para evitar errores
        public Usuario()
        {
            Boletos = new HashSet<Boleto>();
        }

        [Key]
        public int IdUsuario { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        public string NombreCompleto { get; set; } = null!;

        [Required(ErrorMessage = "El correo es obligatorio")]
        [EmailAddress]
        public string Correo { get; set; } = null!;

        public string? Dui { get; set; }

        public string? Telefono { get; set; }

        public string? PasswordHash { get; set; }

        public bool? EsInvitado { get; set; }

        public DateTime? FechaRegistro { get; set; }

        public virtual ICollection<Boleto> Boletos { get; set; }
    }

    public partial class Usuario
    {
     

        public string? Rol { get; set; }

       
    }
}