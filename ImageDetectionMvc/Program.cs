using ImageDetectionMvc.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// Добавление конфигурации
builder.Services.Configure<ApiSettings>(builder.Configuration.GetSection("ApiSettings"));

// Добавление поддержки контроллеров и представлений
builder.Services.AddControllersWithViews();

// Добавление HttpClient
builder.Services.AddHttpClient();

var app = builder.Build();

// Настройка конвейера HTTP-запросов
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
