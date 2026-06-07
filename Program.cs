using AspKnP231.Services.Hash;
using AspKnP231.Middleware.Demo;
using AspKnP231.Services.Scoped;
using AspKnP231.Services.Kdf;
using AspKnP231.Services.Storage;
using AspKnP231.Data;
using Microsoft.EntityFrameworkCore;
using AspKnP231.Middleware.Auth.Session;
using AspKnP231.Middleware.Auth.Token;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Реєструємо співставлення інтерфейсу та класу-сервісу у контейнері
// "якщо буде запит на інжекцію IHashService, то слід видати об'єкт Md5HashService"
// builder.Services.AddSingleton<IHashService, Md5HashService>();
builder.Services.AddHash();   // замінено на розширення (див. HashExtension)
builder.Services.AddKdf();
builder.Services.AddStorage();

builder.Services.AddScoped<ScopedService>();    // без інтерфейсу - тільки один параметр типу

builder.Services.AddDistributedMemoryCache();          // Налаштування сесій
builder.Services.AddSession(options =>                 // https://learn.microsoft.com/en-us/aspnet/core/fundamentals/app-state
{                                                      // 
    options.IdleTimeout = TimeSpan.FromMinutes(10);    // 
    options.Cookie.HttpOnly = true;                    // 
    options.Cookie.IsEssential = true;                 // 
});                                                    // 

// Контекст даних (EF) реєструється як окремий сервіс зі своїми особливостями
builder.Services.AddDbContext<DataContext>(options =>
    options.UseSqlite(
        builder.Configuration.GetConnectionString("MainDb")));

builder.Services.AddScoped<DataAccessor>();
builder.Services.AddCors(options =>
    options.AddDefaultPolicy(policy =>
       policy
       .AllowAnyOrigin()  // або .WithOrigins("http://localhost:5174/", "http://localhost:5175/")
       .AllowAnyHeader()  // або .AllowCredentials() або .WithHeaders("...","...")
       .AllowAnyMethod()  // або .WithMethods("PUT", "...")
));

builder.Services.AddHttpContextAccessor();   // для доступу до HttpContext у сервісах (поза контролерами)

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<DataContext>();
    db.Database.EnsureCreated();
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
app.UseCors();
app.UseAuthorization();
app.MapStaticAssets();
app.UseSession();       // Включення сесій https://learn.microsoft.com/en-us/aspnet/core/fundamentals/app-state

// Місце для обробників користувача (Custom Middlewares)
// порядок оголошення відповідає за порядок зв'язування (послідовності next())
// тому порядок важливо дотримуватись, якщо один обробник залежить від інших
// (на відміну від сервісів, порядок додавання яких не грає ролі)
app.UseDemo();
app.UseAuthSession();
app.UseAuthToken();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();

/* Д.З. Створити декілька іменованих політик CORS
 * Реалізувати підключення однієї з них у відповідності
 * до налаштувань.
 */