using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using CollectionManagement.Data;
using CollectionManagement.Models.Domain;
using CollectionManagement.Services;
using Microsoft.AspNetCore.Identity.UI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<MVCmainDBConetxt>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("devconnection")));

// Configure Identity
builder.Services.AddIdentity<User, Role>(options =>
{
    // Configure password requirements
    options.Password.RequireDigit = false; // Require at least one digit
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false; // Do not require special characters
    options.Password.RequiredLength = 1; // Set your desired length, e.g., 6 characters
    options.Password.RequiredUniqueChars = 0;
})
.AddEntityFrameworkStores<MVCmainDBConetxt>()
.AddDefaultTokenProviders();

builder.Services.AddRazorPages();

// Register the IEmailSender service
builder.Services.AddTransient<IEmailSender, EmailSender>();

//// Add Session support
//builder.Services.AddSession(options =>
//{
//    options.IdleTimeout = TimeSpan.FromMinutes(30); // Set session timeout
//    options.Cookie.HttpOnly = true;
//    options.Cookie.IsEssential = true; // Required for GDPR compliance
//});

var app = builder.Build();

// Seed Data
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var userManager = services.GetRequiredService<UserManager<User>>();
        var roleManager = services.GetRequiredService<RoleManager<Role>>();
        SeedData.Initialize(services, userManager).Wait();
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Add Authentication and Authorization middleware
//app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=PublicItems}/{action=Index}/{id?}");

app.MapRazorPages(); // Ensure Razor Pages are mapped

app.Run();
