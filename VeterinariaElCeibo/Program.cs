using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using VeterinariaElCeibo.Data;
using VeterinariaElCeibo.Models;

var builder = WebApplication.CreateBuilder(args);

// ================== DB CONTEXT ==================
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                       ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// ================== IDENTITY + ROLES ==================
builder.Services
    .AddDefaultIdentity<ApplicationUser>(options =>
    {
        // Para no complicarte con email de confirmación en el TP:
        options.SignIn.RequireConfirmedAccount = false;
    })
    .AddRoles<IdentityRole>() // 👉 soporte de roles
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddControllersWithViews();

var app = builder.Build();

// ================== SEED ROLES + ADMIN ==================
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

    // 1) Crear roles si no existen
    string[] roleNames = { "Administrador", "Veterinario", "Peluqueria" };

    foreach (var roleName in roleNames)
    {
        var roleExists = await roleManager.RoleExistsAsync(roleName);
        if (!roleExists)
        {
            await roleManager.CreateAsync(new IdentityRole(roleName));
        }
    }

    // 2) Crear usuario administrador si no existe
    var adminEmail = "admin@elceibo.com";
    var adminPassword = "Admin123!"; // podés cambiarlo cuando quieras

    var adminUser = await userManager.FindByEmailAsync(adminEmail);
    if (adminUser == null)
    {
        adminUser = new ApplicationUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true // por si después activás RequireConfirmedAccount
        };

        var result = await userManager.CreateAsync(adminUser, adminPassword);
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(adminUser, "Administrador");
        }
        else
        {
            // Podrías loguear los errores, pero para el TP tiramos excepción
            throw new Exception("No se pudo crear el usuario administrador inicial.");
        }
    }
    else
    {
        // Asegurarnos de que tiene el rol Administrador
        if (!await userManager.IsInRoleAsync(adminUser, "Administrador"))
        {
            await userManager.AddToRoleAsync(adminUser, "Administrador");
        }
    }
}

// ================== PIPELINE HTTP ==================
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts(); // HSTS en producción
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); // primero autenticación
app.UseAuthorization();  // luego autorización

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();
