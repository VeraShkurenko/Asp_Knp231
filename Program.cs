using AspKnP231.Services.Hash;
using AspKnP231.Middleware.Demo;
using AspKnP231.Services.Scoped;
using AspKnP231.Services.Kdf;
using AspKnP231.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Реєструємо співставлення інтерфейсу та класу-сервісу у контейнері
// "якщо буде запит на інжекцію IHashService, то слід видати об'єкт Md5HashService"
// builder.Services.AddSingleton<IHashService, Md5HashService>();
builder.Services.AddHash();   // замінено на розширення (див. HashExtension)
builder.Services.AddKdf();

builder.Services.AddScoped<ScopedService>();    // без інтерфейсу - тільки один параметр типу

builder.Services.AddDistributedMemoryCache();          // Налаштування сесій
builder.Services.AddSession(options =>                 // https://learn.microsoft.com/en-us/aspnet/core/fundamentals/app-state
{                                                      // 
    options.IdleTimeout = TimeSpan.FromSeconds(10);    // 
    options.Cookie.HttpOnly = true;                    // 
    options.Cookie.IsEssential = true;                 // 
});                                                    // 

// Контекст даних (EF) реєструється як окремий сервіс зі своїми особливостями
builder.Services.AddDbContext<DataContext>(options =>
    options.UseSqlite(
        builder.Configuration.GetConnectionString("MainDb")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();
app.MapStaticAssets();
app.UseSession();       // Включення сесій https://learn.microsoft.com/en-us/aspnet/core/fundamentals/app-state

// Місце для обробників користувача (Custom Middlewares)
// порядок оголошення відповідає за порядок зв'язування (послідовності next())
// тому порядок важливо дотримуватись, якщо один обробник залежить від інших
// (на відміну від сервісів, порядок додавання яких не грає ролі)
app.UseDemo();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<DataContext>();
    context.Database.EnsureCreated();
}

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();

/* Д.З. Створити сторінку для обчислення DK *Derived Key*
 * Користувач вводить сіль та пароль, натискає кнопку "обчислити"
 * і одержує результат.
 * ** Додати режим автоматичної герерації солі
 */