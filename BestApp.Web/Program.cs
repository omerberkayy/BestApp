using Microsoft.EntityFrameworkCore;
using BestApp.Data;
using BestApp.Data.Repositories;
using BestApp.Core.Repositories;
using BestApp.Core.Services;
using BestApp.Service.Services;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

    builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Service Kayıtları
builder.Services.AddScoped(typeof(IService<>), typeof(Service<>));
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<IUserService, UserManager>();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "PeyzajAdminAuth";
        options.LoginPath = "/Admin/Auth/Login";
        options.AccessDeniedPath = "/Admin/Auth/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
    });

var app = builder.Build();

// 1. GÜVENLİ DATA SEEDING (Çalışma Zamanında İlk Admini Oluşturma)
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<BestApp.Data.AppDbContext>();
    var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();

    context.Database.Migrate(); // Bekleyen migration varsa uygular

    if (!context.AppUsers.Any())
    {
        var adminUser = configuration["AdminSettings:Username"] ?? "admin";
        var adminPass = configuration["AdminSettings:Password"] ?? "123456";

        context.AppUsers.Add(new BestApp.Core.Entities.AppUser
        {
            Username = adminUser,
            PasswordHash = BestApp.Core.Utilities.PasswordHelper.HashPassword(adminPass),
            Role = "Admin",
            IsActive = true
        });
        context.SaveChanges();
    }
    // 2. HAKKIMIZDA VERİSİNİ YÜKLEME 
    if (!context.Abouts.Any())
    {
        context.Abouts.Add(new BestApp.Core.Entities.About
        {
            Title = "Doğayı Yaşam Alanlarınıza Taşıyoruz",
            Content = "Best Peyzaj olarak Adana ve çevresinde, estetik ve fonksiyonelliği bir araya getiren peyzaj projeleri üretiyoruz.",
            ImageUrl = "https://images.unsplash.com/photo-1558904541-efa843a96f09?q=80&w=2000&auto=format&fit=crop"
        });
        context.SaveChanges();
    }

    // 3. HİZMETLER VERİSİNİ YÜKLEME 
    if (!context.Services.Any())
    {
        context.Services.AddRange(
            new BestApp.Core.Entities.Service
            {
                Title = "Peyzaj Tasarımı",
                Description = "Villalar, siteler ve ticari alanlar için 3D modelleme destekli profesyonel peyzaj proje çizimleri ve tasarımları.",
                IconClass = "bi-palette", // Bootstrap icon
                IsActive = true
            },
            new BestApp.Core.Entities.Service
            {
                Title = "Bahçe Bakımı & Düzenleme",
                Description = "Mevcut bahçelerinizin periyodik bakımı, çim biçme, gübreleme, ilaçlama ve budama hizmetleri.",
                IconClass = "bi-scissors",
                IsActive = true
            },
            new BestApp.Core.Entities.Service
            {
                Title = "Otomatik Sulama Sistemleri",
                Description = "Su tasarrufu sağlayan, bahçenizin ihtiyacına uygun akıllı damlama ve fıskiye sulama sistemleri kurulumu.",
                IconClass = "bi-droplet-fill",
                IsActive = true
            }
        );
        context.SaveChanges();
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapAreaControllerRoute(
    name: "admin",
    areaName: "Admin",
    pattern: "Admin/{controller=Home}/{action=Index}/{id?}");
    
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
