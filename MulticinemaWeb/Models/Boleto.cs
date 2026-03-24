using System;
using System.Collections.Generic;

namespace MulticinemaWeb.Models;

public partial class Boleto
{
    public int IdBoleto { get; set; }

    public int IdFuncion { get; set; }

    public int IdUsuario { get; set; }

    public string AsientoCodigo { get; set; } = null!;

    public decimal PrecioPagado { get; set; }

    public string? CodigoQr { get; set; }

    public string? Estado { get; set; }

    public DateTime? FechaCompra { get; set; }

    public virtual Funcione IdFuncionNavigation { get; set; } = null!;

    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;
}
