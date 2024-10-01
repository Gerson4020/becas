using BECAS.Interfaces;
using BECASLC;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Azure.Identity;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using BECAS.Models;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("BecasDatabase");
builder.Services.AddDbContext<MEOBContext>(options => options.UseSqlServer(connectionString));
builder.Services.AddScoped<ICatalogos, CatalogosRepository>();
builder.Services.AddScoped<IPersonas, PersonasRepository>();
builder.Services.Configure<EncryptionSettings>(builder.Configuration.GetSection("EncryptionSettings"));
builder.Services.AddScoped<IEncryptionService, EncryptionService>();
// Add services to the container.
builder.Services.AddRazorPages();

// Configure multiple authentication schemes
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = "ApplicationScheme"; // Login personalizado como default
    options.DefaultChallengeScheme = "AzureADScheme"; // AD como fallback
})
.AddCookie("ApplicationScheme", options =>
{
    options.LoginPath = "/Auth/Login"; // Ruta para login personalizado
})
.AddMicrosoftIdentityWebApp(builder.Configuration.GetSection("AzureAd"), "AzureADScheme");

// Configurar políticas de autorización
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ADOnly", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.AddAuthenticationSchemes("AzureADScheme");
    });

    options.AddPolicy("ExternalUsers", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.AddAuthenticationSchemes("ApplicationScheme");
    });
});

// Configurar MVC y aplicar la política de AD por defecto
builder.Services.AddMvc(option =>
{
    var adPolicy = new AuthorizationPolicyBuilder("AzureADScheme")
                    .RequireAuthenticatedUser()
                    .Build();

    // Aplicar la política por defecto a todas las rutas, pero puedes usar otros
    // atributos [Authorize] en controladores o acciones específicas para otra política
    option.Filters.Add(new AuthorizeFilter(adPolicy));

}).AddMicrosoftIdentityUI();

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromSeconds(30000);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}
app.UseDeveloperExceptionPage();
app.UseStatusCodePages();
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.UseSession();
app.MapRazorPages();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
