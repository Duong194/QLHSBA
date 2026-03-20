using DACS_web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// 🛠️ Cấu hình DbContext đúng vị trí trước khi Build
var useInMemory = Environment.GetEnvironmentVariable("USE_INMEMORY_DB") == "true";

if (useInMemory)
{
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseInMemoryDatabase("DACS_Demo"));
}
else
{
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
}


// Cấu hình Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddDefaultTokenProviders()
    .AddDefaultUI()
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = $"/Identity/Account/Login";
    options.LogoutPath = $"/Identity/Account/Logout";
    options.AccessDeniedPath = $"/Identity/Account/AccessDenied";
});

builder.Services.AddRazorPages();

var app = builder.Build(); // ✨ Build sau khi đã Add Services

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseStaticFiles();

app.UseAuthorization();

app.MapStaticAssets();

app.MapRazorPages();
app.MapControllerRoute(
    name: "Admin",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    var db = services.GetRequiredService<ApplicationDbContext>();

    db.Database.EnsureCreated();

    // Seed Roles
    var roles = new[]
    {
        new IdentityRole { Id = "49b0daae-1996-46cd-9a01-a63785f78666", Name = "YTa", NormalizedName = "YTA" },
        new IdentityRole { Id = "49c7f2d2-fbbe-4d10-8341-633d755cb7c6", Name = "BenhNhan", NormalizedName = "BENHNHAN" },
        new IdentityRole { Id = "74f7f7c7-f1a6-42be-bfe3-04d536c37d5a", Name = "Admin", NormalizedName = "ADMIN" },
        new IdentityRole { Id = "a4470d03-67c0-4d9e-a008-7396139f79e5", Name = "BacSi", NormalizedName = "BACSI" },
    };
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role.Name))
            await roleManager.CreateAsync(role);
    }

    // Seed Users
    async Task SeedUser(string id, string email, string password, string roleId)
    {
        if (await userManager.FindByIdAsync(id) == null)
        {
            var user = new ApplicationUser
            {
                Id = id,
                UserName = email,
                NormalizedUserName = email.ToUpper(),
                Email = email,
                NormalizedEmail = email.ToUpper(),
                EmailConfirmed = true,
                LockoutEnabled = true,
            };
            await userManager.CreateAsync(user, password);
            var role = roles.First(r => r.Id == roleId);
            await userManager.AddToRoleAsync(user, role.Name);
        }
    }

    await SeedUser("fcb8920d-c1bb-42a9-9d6b-57c14be6b99e", "yta1@gmail.com", "YTa@123", "49b0daae-1996-46cd-9a01-a63785f78666");
    await SeedUser("b6dccc35-7208-4fdc-af53-3ff01b4993f0", "duong1@gmail.com", "BenhNhan@123", "49c7f2d2-fbbe-4d10-8341-633d755cb7c6");
    await SeedUser("b9b5dfea-c0b6-4dd5-957c-ce03b204b1b4", "bn3@gmail.com", "BenhNhan@123", "49c7f2d2-fbbe-4d10-8341-633d755cb7c6");
    await SeedUser("94f019f7-a3f3-4ef9-8c39-b3f35e0362b2", "admin2@gmail.com", "Admin@123", "74f7f7c7-f1a6-42be-bfe3-04d536c37d5a");
    await SeedUser("b5306f74-1bfb-4674-9d45-86f0c77aa90c", "bs2@gmail.com", "BacSi@123", "a4470d03-67c0-4d9e-a008-7396139f79e5");
    await SeedUser("c77506e1-621d-41be-a5a6-ce6177ad504b", "bs1@gmail.com", "BacSi@123", "a4470d03-67c0-4d9e-a008-7396139f79e5");
}
await app.RunAsync();
