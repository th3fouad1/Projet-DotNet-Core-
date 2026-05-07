using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ASVS_Security_Auditor.Infrastructure;
using ASVS_Security_Auditor.Infrastructure.Data;
using ASVS_Security_Auditor.Infrastructure.Parsers;
using ASVS_Security_Auditor.Core.Interfaces;
using ASVS_Security_Auditor.Core.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();

builder.Services.AddHttpClient();
builder.Services.AddInfrastructureServices(builder.Configuration);

var app = builder.Build();

// Seed database with ASVS requirements and Identity Roles
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    
    // Seed ASVS Data
    if (!context.AsvsRequirements.Any())
    {
        var importService = scope.ServiceProvider.GetRequiredService<IAsvsImportService>();
        var filePath = @"D:\dotnetProjet\ASVS 4.0 Checklist (template) (2).xlsx";
        if (System.IO.File.Exists(filePath))
        {
            using var stream = System.IO.File.OpenRead(filePath);
            importService.ImportFromExcelAsync(stream).Wait();
        }
    }

    // Seed Roles and Admin User
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

    if (!roleManager.RoleExistsAsync("Admin").GetAwaiter().GetResult())
    {
        roleManager.CreateAsync(new IdentityRole("Admin")).GetAwaiter().GetResult();
    }
    if (!roleManager.RoleExistsAsync("Auditor").GetAwaiter().GetResult())
    {
        roleManager.CreateAsync(new IdentityRole("Auditor")).GetAwaiter().GetResult();
    }

    var adminEmail = "admin@test.com";
    var adminUser = userManager.FindByEmailAsync(adminEmail).GetAwaiter().GetResult();
    if (adminUser == null)
    {
        adminUser = new IdentityUser { UserName = adminEmail, Email = adminEmail, EmailConfirmed = true };
        userManager.CreateAsync(adminUser, "Password123!").GetAwaiter().GetResult();
        userManager.AddToRoleAsync(adminUser, "Admin").GetAwaiter().GetResult();
    }
    else if (!userManager.IsInRoleAsync(adminUser, "Admin").GetAwaiter().GetResult())
    {
        userManager.AddToRoleAsync(adminUser, "Admin").GetAwaiter().GetResult();
    }
}

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
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapRazorPages()
   .WithStaticAssets();

app.Run();