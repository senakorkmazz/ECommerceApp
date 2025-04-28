using ECommerceApp.Services;
using ECommerceApp.Data;
using ECommerceApp.Models;
using ECommerceApp.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options; // IOptions için

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// --- MSSQL DbContext Configuration ---
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// --- MongoDB Configuration ---
// appsettings.json'dan MongoDbSettings bölümünü oku ve MongoDbSettings sýnýfýna map'le
builder.Services.Configure<MongoDbSettings>(
    builder.Configuration.GetSection("MongoDbSettings"));

// MongoDB Servisini Singleton olarak ekle (veya Scoped/Transient ihtiyaca göre)
// IOptions<MongoDbSettings> baðýmlýlýðýný otomatik çözer.
builder.Services.AddScoped<ProductService>();
// Veya AddScoped<ProductMongoService>();

// --- Controller ve View Servisleri ---
builder.Services.AddControllersWithViews();

// --- Session Yönetimi (Login durumu için basit yöntem) ---
builder.Services.AddDistributedMemoryCache(); // Session için bellek tabanlý saklama alaný
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Oturum süresi
    options.Cookie.HttpOnly = true; // Cookie'ye sadece HTTP üzerinden eriþilsin
    options.Cookie.IsEssential = true; // GDPR uyumluluðu için gerekli
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); // Eðer Identity kullanýlsaydý gerekli olurdu
app.UseAuthorization();

// --- Session middleware'ýný ekle ---
app.UseSession(); // UseRouting'den sonra, UseEndpoints/MapControllerRoute'dan önce olmalý

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();