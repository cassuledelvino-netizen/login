using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using login.Data;
using login.Services;

var builder = WebApplication.CreateBuilder(args);

// 1. Configurar conexão com SQLite
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException(
        "Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// 2. Configurar ASP.NET Core Identity
builder.Services.AddDefaultIdentity<IdentityUser>(
    options =>
    {
        options.SignIn.RequireConfirmedAccount = true;

        // Configurações de senha
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireNonAlphanumeric = true;
        options.Password.RequiredLength = 8;

        // Proteção contra bloqueio por várias tentativas
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
        options.Lockout.MaxFailedAccessAttempts = 5;
        options.Lockout.AllowedForNewUsers = true;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>();

// 3. Configurar o serviço de e-mail do Brevo
builder.Services.AddTransient<
    IEmailSender<IdentityUser>,
    BrevoEmailSender>();

// 4. MVC
builder.Services.AddControllersWithViews();

var app = builder.Build();

// 5. Middleware
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// 6. Rotas MVC
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// 7. Rotas Razor Pages / Identity
app.MapRazorPages();

app.Run();