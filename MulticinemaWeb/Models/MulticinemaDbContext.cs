using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace MulticinemaWeb.Models;

public partial class MulticinemaDbContext : DbContext
{
    public MulticinemaDbContext()
    {
    }

    public MulticinemaDbContext(DbContextOptions<MulticinemaDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Boleto> Boletos { get; set; }

    public virtual DbSet<Funcione> Funciones { get; set; }

    public virtual DbSet<Pelicula> Peliculas { get; set; }

    public virtual DbSet<Sala> Salas { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
      //  => optionsBuilder.UseSqlServer("Server=LAPTOP-K4QF0EGQ;Database=MulticinemaDB;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Boleto>(entity =>
        {
            entity.HasKey(e => e.IdBoleto).HasName("PK__Boletos__362F6EA07C8CD429");

            entity.Property(e => e.AsientoCodigo).HasMaxLength(10);
            entity.Property(e => e.CodigoQr).HasColumnName("CodigoQR");
            entity.Property(e => e.Estado)
                .HasMaxLength(20)
                .HasDefaultValue("Pagado");
            entity.Property(e => e.FechaCompra)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.PrecioPagado).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.IdFuncionNavigation).WithMany(p => p.Boletos)
                .HasForeignKey(d => d.IdFuncion)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Boletos_Funciones");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.Boletos)
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Boletos_Usuarios");
        });

        modelBuilder.Entity<Funcione>(entity =>
        {
            entity.HasKey(e => e.IdFuncion).HasName("PK__Funcione__7D413288DC3033F0");

            entity.Property(e => e.FechaHoraInicio).HasColumnType("datetime");
            entity.Property(e => e.Precio).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.IdPeliculaNavigation).WithMany(p => p.Funciones)
                .HasForeignKey(d => d.IdPelicula)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Funciones_Peliculas");

            entity.HasOne(d => d.IdSalaNavigation).WithMany(p => p.Funciones)
                .HasForeignKey(d => d.IdSala)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Funciones_Salas");
        });

        modelBuilder.Entity<Pelicula>(entity =>
        {
            entity.HasKey(e => e.IdPelicula).HasName("PK__Pelicula__60537FD05B34492D");

            entity.Property(e => e.Clasificacion).HasMaxLength(10);
            entity.Property(e => e.Estado).HasMaxLength(20);
            entity.Property(e => e.PosterUrl).HasMaxLength(500);
            entity.Property(e => e.Titulo).HasMaxLength(150);
            entity.Property(e => e.TrailerUrl).HasMaxLength(500);
        });

        modelBuilder.Entity<Sala>(entity =>
        {
            entity.HasKey(e => e.IdSala).HasName("PK__Salas__A04F9B3B881A08A8");

            entity.Property(e => e.NombreSala).HasMaxLength(50);
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.IdUsuario).HasName("PK__Usuarios__5B65BF97F6149117");

            entity.Property(e => e.Correo).HasMaxLength(100);
            entity.Property(e => e.Dui).HasMaxLength(10);
            entity.Property(e => e.EsInvitado).HasDefaultValue(false);
            entity.Property(e => e.FechaRegistro)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.NombreCompleto).HasMaxLength(100);
            entity.Property(e => e.PasswordHash).HasMaxLength(255);
            entity.Property(e => e.Telefono).HasMaxLength(15);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
