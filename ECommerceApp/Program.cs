using ECommerceApp.Services;
using ECommerceApp.Data;
using ECommerceApp.Models;
using ECommerceApp.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options; // IOptions i�in

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// --- MSSQL DbContext Configuration ---
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// --- MongoDB Configuration ---
// appsettings.json'dan MongoDbSettings b�l�m�n� oku ve MongoDbSettings s�n�f�na map'le
builder.Services.Configure<MongoDbSettings>(
    builder.Configuration.GetSection("MongoDbSettings"));

// MongoDB Servisini Singleton olarak ekle (veya Scoped/Transient ihtiyaca g�re)
// IOptions<MongoDbSettings> ba��ml�l���n� otomatik ��zer.
builder.Services.AddScoped<ProductService>();
// Veya AddScoped<ProductMongoService>();

// --- Controller ve View Servisleri ---
builder.Services.AddControllersWithViews();

// --- Session Y�netimi (Login durumu i�in basit y�ntem) ---
builder.Services.AddDistributedMemoryCache(); // Session i�in bellek tabanl� saklama alan�
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Oturum s�resi
    options.Cookie.HttpOnly = true; // Cookie'ye sadece HTTP �zerinden eri�ilsin
    options.Cookie.IsEssential = true; // GDPR uyumlulu�u i�in gerekli
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

app.UseAuthentication(); // E�er Identity kullan�lsayd� gerekli olurdu
app.UseAuthorization();

// --- Session middleware'�n� ekle ---
app.UseSession(); // UseRouting'den sonra, UseEndpoints/MapControllerRoute'dan �nce olmal�

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();