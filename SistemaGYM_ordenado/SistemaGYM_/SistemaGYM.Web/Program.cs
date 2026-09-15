using SistemaGYM.Web.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Sesión simple en servidor para trackear quién está logueado y con qué rol
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(4);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddHttpContextAccessor();

builder.Services.AddHttpClient("GymApi", client =>
{
    client.BaseAddress = new Uri("http://localhost:5215/api/");
});

// Servicios que consumen la API
builder.Services.AddScoped<IAlumnoApiService, AlumnoApiService>();
builder.Services.AddScoped<IProfesorApiService, ProfesorApiService>();
builder.Services.AddScoped<IActividadApiService, ActividadApiService>();
builder.Services.AddScoped<ISuscripcionApiService, SuscripcionApiService>();
builder.Services.AddScoped<IAlumnoSuscripcionApiService, AlumnoSuscripcionApiService>();
builder.Services.AddScoped<IPagoApiService, PagoApiService>();
builder.Services.AddScoped<IRutinaApiService, RutinaApiService>();
builder.Services.AddScoped<IAlimentacionApiService, AlimentacionApiService>();
builder.Services.AddScoped<IAnuncioApiService, AnuncioApiService>();
builder.Services.AddScoped<IAuthApiService, AuthApiService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseSession();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Auth}/{action=Login}/{id?}")
    .WithStaticAssets();

app.Run();
