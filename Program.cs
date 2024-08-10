using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using metalduck.Data;
using Microsoft.AspNetCore.Authorization;
using metalduck.Authorization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddRazorPages();

builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});

builder.Services.AddScoped<IAuthorizationHandler, DocumentIsOwnerAuthorizationHandler>();
builder.Services.AddSingleton<IAuthorizationHandler, DocumentAdministratorsAuthorizationHandler>();
builder.Services.AddSingleton<IAuthorizationHandler, DocumentManagerAuthorizationHandler>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

using(var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    var roles = new[] { "Admin", "Manager", "Member" };

    foreach(var role in roles)
    {
        if(!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }
}

using (var scope = app.Services.CreateScope())
{
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

    string adminEmail = "admin@metalduck.com";
    string adminPassword = "Admin@123";

    if (await userManager.FindByEmailAsync(adminEmail) == null)
    {
        var user = new IdentityUser();
        user.UserName = adminEmail;
        user.Email = adminEmail;

        await userManager.CreateAsync(user, adminPassword);
        await userManager.AddToRoleAsync(user, Constants.DocumentAdministratorsRole);
    }

    string managerEmail = "manager@metalduck.com";
    string managerPassword = "Manager@123";

    if (await userManager.FindByEmailAsync(managerEmail) == null)
    {
        var user = new IdentityUser();
        user.UserName = managerEmail;
        user.Email = managerEmail;

        await userManager.CreateAsync(user, managerPassword);
        await userManager.AddToRoleAsync(user, Constants.DocumentManagersRole);
    }
}

app.Run();