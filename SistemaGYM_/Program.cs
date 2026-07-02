using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using SistemaGYM.Datos;
using SistemaGYM.Endpoints;
using SistemaGYM.Logica;
using SistemaGYM.Repositorios;
using SistemaGYM.Middleware; 
using SistemaGYM.Entidades;

var builder = WebApplication.CreateBuilder(args);

// Servicios :)

builder.Services.AddOpenApi();
builder.Services.AddValidation();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

// DbContext cadena de conexion!!
builder.Services.AddDbContext<GimnasioContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Repositorios
builder.Services.AddScoped<IAlumnoRepository, AlumnoRepository>();
builder.Services.AddScoped<IProfesorRepository, ProfesorRepository>();
builder.Services.AddScoped<IActividadRepository, ActividadRepository>();
builder.Services.AddScoped<IActividadAlumnoRepository, ActividadAlumnoRepository>();
builder.Services.AddScoped<IAlimentacionRepository, AlimentacionRepository>();
builder.Services.AddScoped<IAnuncioRepository, AnuncioRepository>();
builder.Services.AddScoped<IPagoRepository, PagoRepository>();
builder.Services.AddScoped<IRutinaRepository, RutinaRepository>();
builder.Services.AddScoped<ISuscripcionRepository, SuscripcionRepository>();
builder.Services.AddScoped<IAlumnoSuscripcionRepository, AlumnoSuscripcionRepository>();
builder.Services.AddScoped<IAuthLogica, AuthLogica>();

// Lógica de negocio
builder.Services.AddScoped<IAlumnoLogica, AlumnoLogica>();
builder.Services.AddScoped<IProfesorLogica, ProfesorLogica>();
builder.Services.AddScoped<IActividadLogica, ActividadLogica>();
builder.Services.AddScoped<IActividadAlumnoLogica, ActividadAlumnoLogica>();
builder.Services.AddScoped<IAlimentacionLogica, AlimentacionLogica>();
builder.Services.AddScoped<IAnuncioLogica, AnuncioLogica>();
builder.Services.AddScoped<IPagoLogica, PagoLogica>();
builder.Services.AddScoped<IRutinaLogica, RutinaLogica>();
builder.Services.AddScoped<ISuscripcionLogica, SuscripcionLogica>();
builder.Services.AddScoped<IAlumnoSuscripcionLogica, AlumnoSuscripcionLogica>();

// CORS 
builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendPolicy", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Middleware / Pipeline 
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(); // UI de documentación en /scalar/v1
}

app.UseHttpsRedirection();
app.UseCors("FrontendPolicy");

// Endpoints 

app.MapAlumnoEndpoints();
app.MapProfesorEndpoints();
app.MapActividadEndpoints();
app.MapActividadAlumnoEndpoints();
app.MapAlimentacionEndpoints();
app.MapAnuncioEndpoints();
app.MapPagoEndpoints();
app.MapRutinaEndpoints();
app.MapSuscripcionEndpoints();
app.MapAlumnoSuscripcionEndpoints();
app.MapAuthEndpoints();

app.Run();
