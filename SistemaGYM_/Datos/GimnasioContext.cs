using Microsoft.EntityFrameworkCore;
using SistemaGYM.Datos;
using SistemaGYM.Entidades;
using SistemaGYM.Endpoints;
using SistemaGYM.Middleware;
using SistemaGYM.Logica;
using SistemaGYM.Repositorios;

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
    public DbSet<ActividadAlumno> ActividadesAlumno { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Usuario>()
            .HasDiscriminator<string>("TipoUsuario")
            .HasValue<Alumno>("Alumno")
            .HasValue<Profesor>("Profesor");

        modelBuilder.Entity<Pago>()
            .Property(p => p.MetodoPago)
            .HasConversion<string>();


        modelBuilder.Entity<Actividad>()
            .Property(a => a.Dias)
            .HasConversion<string>();

        modelBuilder.Entity<Administrador>().HasData(new Administrador
        {
        Id = 1,
        Usuario = "Admin123",
        //Contraseña123,este es solo es Hash!!!!
        //PBKDF2-HMAC-SHA256, salt de 16 bytes, hash de 32 bytes, 100.000 iteraciones
        Contrasenia = "/7cq1tNYOK1W9U+6SEyBkA==.Z/OMk9+ZDCsUNaT8Z0Ysy9vW9w8lJZU9GwVgTRhdM4o="
        });

        modelBuilder.Entity<Rutina>()
            .HasOne(r => r.Profesor)
            .WithMany(p => p.Rutinas)
            .HasForeignKey(r => r.ProfesorId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Rutina>()
        .HasOne(r => r.Alumno)
        .WithMany() 
        .HasForeignKey(r => r.AlumnoId)
        .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Actividad>()
            .HasOne(a => a.Profesor)
            .WithMany(p => p.Actividades)
            .HasForeignKey(a => a.ProfesorId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Pago>()
            .HasOne(p => p.Alumno)
            .WithMany(a => a.AlumnoPagos)
            .HasForeignKey(p => p.AlumnoId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Alimentacion>()
            .HasOne(a => a.Profesor)
            .WithMany(p => p.PlanesAlimentacion)
            .HasForeignKey(a => a.ProfesorId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Anuncio>()
            .HasOne(a => a.Profesor)
            .WithMany(p => p.Anuncios)
            .HasForeignKey(a => a.ProfesorId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}