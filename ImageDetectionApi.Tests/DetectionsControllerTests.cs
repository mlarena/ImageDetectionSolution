using ImageDetectionApi.Controllers;
using ImageDetectionApi.Data;
using ImageDetectionApi.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

public class DetectionsControllerTests : IClassFixture<WebApplicationFactory<ImageDetectionApi.Program>>
{
    private readonly WebApplicationFactory<ImageDetectionApi.Program> _factory;
    private readonly HttpClient _client;

    public DetectionsControllerTests(WebApplicationFactory<ImageDetectionApi.Program> factory)
    {
        _factory = factory;
        _client = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
                if (descriptor != null) services.Remove(descriptor);

                services.AddDbContext<AppDbContext>(options =>
                {
                    var configuration = services.BuildServiceProvider().GetRequiredService<IConfiguration>();
                    options.UseSqlite(configuration.GetConnectionString("DefaultConnection"));
                });

                var sp = services.BuildServiceProvider();

                using (var scope = sp.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    var db = scopedServices.GetRequiredService<AppDbContext>();
                    var logger = scopedServices.GetRequiredService<ILogger<DetectionsControllerTests>>();

                    db.Database.EnsureCreated();

                    try
                    {
                        // Добавление 100 записей
                        for (int i = 0; i < 100; i++)
                        {
                            db.Detections.Add(new Detection
                            {
                                Title = $"Test Detection {i}",
                                ImageName = $"image{i}.jpg",
                                VideoName = $"video{i}.mp4",
                                ClassName = $"class{i}",
                                Latitude = 52.53 + (i * 0.01),
                                Longitude = 54.54 + (i * 0.01),
                                Status = "active",
                                CriticalLevel = i % 10,
                                DateTimeDetection = DateTime.Now.AddDays(i)
                            });
                        }
                        db.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "An error occurred seeding the database with test messages. Error: {Message}", ex.Message);
                    }
                }
            });
        }).CreateClient();
    }

    [Fact]
    public async Task GetDetections_ReturnsSuccess()
    {
        var response = await _client.GetAsync("/api/detections");
        response.EnsureSuccessStatusCode();
        Assert.Equal("application/json; charset=utf-8", response.Content.Headers.ContentType.ToString());

        var detections = await response.Content.ReadFromJsonAsync<IEnumerable<Detection>>();
        Assert.Equal(100, detections?.Count());
    }

    [Fact]
    public async Task PostDetection_ReturnsSuccess()
    {
        var detection = new Detection
        {
            Title = "Test Detection",
            ImageName = "image.jpg",
            VideoName = "video.mp4",
            ClassName = "class1",
            Latitude = 52.53,
            Longitude = 54.54,
            Status = "active",
            CriticalLevel = 1,
            DateTimeDetection = DateTime.Now
        };

        var json = JsonSerializer.Serialize(detection);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await _client.PostAsync("/api/detections", content);
        response.EnsureSuccessStatusCode();
    }
}
