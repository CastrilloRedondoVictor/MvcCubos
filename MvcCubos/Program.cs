using Microsoft.EntityFrameworkCore;
using MvcCubos.Data;
using MvcCubos.Repositories;
using MvcNetCoreUtilidades.Helpers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

/*****************************************************************************************************************************************/
string connectionString = builder.Configuration.GetConnectionString("MySqlCubos");
builder.Services.AddDbContext<CubosContext>(x => x.UseMySQL(connectionString));
builder.Services.AddTransient<RepositoryCubos>();

builder.Services.AddSingleton<HelperPathProvider>();
builder.Services.AddHttpContextAccessor();
/*****************************************************************************************************************************************/

// Configurar la sesi�n
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

/*****************************************************************************************************************************************/
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
/**********************************************************************************************************************************************/
app.UseStaticFiles();
/**********************************************************************************************************************************************/
// Habilitar la sesi�n
app.UseSession(); // Habilitar el middleware de sesi�n

app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();