using System;
using System.Collections.Generic;

namespace MulticinemaWeb.Models;

public partial class Sala
{
    public int IdSala { get; set; }

    public string NombreSala { get; set; } = null!;

    public int CapacidadTotal { get; set; }

    public string? DistribucionAsientos { get; set; }

    public virtual ICollection<Funcione> Funciones { get; set; } = new List<Funcione>();
}
