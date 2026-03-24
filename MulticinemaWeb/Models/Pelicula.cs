using System;
using System.Collections.Generic;

namespace MulticinemaWeb.Models;

public partial class Pelicula
{
    public int IdPelicula { get; set; }

    public string Titulo { get; set; } = null!;

    public string? Sinopsis { get; set; }

    public int DuracionMinutos { get; set; }

    public string? Clasificacion { get; set; }

    public string? PosterUrl { get; set; }

    public string? TrailerUrl { get; set; }

    public string? Estado { get; set; }

    public virtual ICollection<Funcione> Funciones { get; set; } = new List<Funcione>();
}
