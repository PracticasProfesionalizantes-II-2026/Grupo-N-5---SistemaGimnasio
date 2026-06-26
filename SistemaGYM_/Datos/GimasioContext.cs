using Microsoft.EntityFrameworkCore;
using SistemaGYM.Entidades;
using System.Security.Cryptography;

namespace SistemaGYM.Datos;

public class GimnasioContext(DbContextOptions<GimnasioContext> options) : DbContext(options)
{
    public DbSet<Alumno> Alumnos { get; set; }
    public DbSet<Profesor> Profesores { get; set; }
    public DbSet<Administrador> Administradores { get; set; }
    public DbSet<Actividad> Actividades { get; set; }
    public DbSet<Alimentacion> Alimentaciones { get; set; }
    public DbSet<Anuncio> Anuncios { get; set; }
    public DbSet<Pago> Pagos { get; set; }
    public DbSet<Rutina> Rutinas { get; set; }
    public DbSet<Suscripcion> Suscripciones { get; set; }
    public DbSet<AlumnoSuscripcion> AlumnoSuscripciones { get; set; }


protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<Usuario>()
        .HasDiscriminator<string>("TipoUsuario")
        .HasValue<Alumno>("Alumno")
        .HasValue<Profesor>("Profesor")
        .HasValue<Administrador>("Administrador");

    modelBuilder.Entity<Pago>()
        .Property(p => p.MetodoPago)
        .HasConversion<string>();

    modelBuilder.Entity<Administrador>().HasData(new Administrador
    {
        Id = 1,
        Usuario = "Admin123",
        Contraseña = "Contraseña123"

    });
}
}