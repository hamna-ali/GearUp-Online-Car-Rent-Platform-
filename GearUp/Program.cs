using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using GearUp.Data;
using GearUp.Models.Interfaces;
using GearUp.Models.Repositories;
using GearUp.Models;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddRazorPages();
builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.ConfigureApplicationCookie(options =>
{
    options.AccessDeniedPath = "/Account/AccessDenied";
});
builder.Services.AddControllersWithViews();

builder.Services.AddScoped<ICartRepository, CartRepository>();

builder.Services.AddScoped<IVehicleRepository, VehicleRepository>();
builder.Services.AddScoped<IBookingRepository, BookingRepository>();

builder.Services.AddSignalR();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("IsAdmin", policy =>
        policy.RequireClaim("Permission", "AdminAccess"));

    options.AddPolicy("IsUser", policy =>
        policy.RequireClaim("Permission", "UserAccess"));
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
async Task SeedRolesAsync(IServiceProvider services)
{
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

    string[] roleNames = { "Admin", "User" };
    foreach (var role in roleNames)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
            Console.WriteLine($"Role '{role}' created."); // ? Add this
        }
        else
        {
            Console.WriteLine($"Role '{role}' already exists."); // ? And this
        }

    }
}
async Task SeedClaimsAsync(IServiceProvider services)
{
    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

    var adminUser = await userManager.FindByEmailAsync("admin@example.com"); // Use real email or loop users by role
    if (adminUser != null)
    {
        var claims = await userManager.GetClaimsAsync(adminUser);
        if (!claims.Any(c => c.Type == "Permission" && c.Value == "AdminAccess"))
        {
            await userManager.AddClaimAsync(adminUser, new Claim("Permission", "AdminAccess"));
        }
    }

    var normalUser = await userManager.FindByEmailAsync("user@example.com"); // Change as needed
    if (normalUser != null)
    {
        var claims = await userManager.GetClaimsAsync(normalUser);
        if (!claims.Any(c => c.Type == "Permission" && c.Value == "UserAccess"))
        {
            await userManager.AddClaimAsync(normalUser, new Claim("Permission", "UserAccess"));
        }
    }
}

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await SeedRolesAsync(services);
    await SeedClaimsAsync(services); // <- ADD THIS
}

app.MapHub<NotificationHub>("/notificationHub");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapGet("/", context =>
{
    context.Response.Redirect("/Identity/Account/Login");
    return Task.CompletedTask;
});

app.MapRazorPages();

app.Run();

