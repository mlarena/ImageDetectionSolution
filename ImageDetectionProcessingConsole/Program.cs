using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace ImageDetectionProcessingConsole
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Image Detection Processing Console Application");

            // Настройка DI-контейнера
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            var serviceProvider = serviceCollection.BuildServiceProvider();

            // Получение HttpClient из DI-контейнера
            var httpClient = serviceProvider.GetRequiredService<HttpClient>();

            // Пример: Загрузка JSON данных из файла
            string jsonFilePath = @"C:\_dotnet\ImageDetectionSolution\ImageDetectionProcessingConsole\Front.json";

            if (File.Exists(jsonFilePath))
            {
                string jsonContent = await File.ReadAllTextAsync(jsonFilePath);
                var detections = JsonSerializer.Deserialize<List<Detection>>(jsonContent);

                foreach (var detection in detections)
                {
                    Console.WriteLine($"Sending detection data: {detection.Title}");
                    await SendDetectionDataAsync(httpClient, detection);
                }
            }
            else
            {
                Console.WriteLine($"JSON file does not exist: {jsonFilePath}");
            }
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpClient();
        }

        private static async Task SendDetectionDataAsync(HttpClient httpClient, Detection detection)
        {
            var apiUrl = "http://localhost:5070/api/detections";

            // Преобразование Latitude и Longitude в double
            if (double.TryParse(detection.Latitude, out double latitude) &&
                double.TryParse(detection.Longitude, out double longitude))
            {
                var detectionToSend = new Detection
                {
                    Id = detection.Id,
                    Title = detection.Title,
                    ImageName = detection.ImageName,
                    VideoName = detection.VideoName,
                    ClassName = detection.ClassName,
                    Latitude = latitude.ToString(), // Преобразование обратно в строку для отправки
                    Longitude = longitude.ToString(), // Преобразование обратно в строку для отправки
                    Status = detection.Status,
                    CriticalLevel = detection.CriticalLevel,
                    DateTimeDetection = detection.DateTimeDetection
                };

                var response = await httpClient.PostAsJsonAsync(apiUrl, detectionToSend);

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Detection data sent successfully.");
                }
                else
                {
                    Console.WriteLine($"Failed to send detection data. Status code: {response.StatusCode}");
                }
            }
            else
            {
                Console.WriteLine("Failed to parse Latitude or Longitude.");
            }
        }
    }

    public class Detection
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string ImageName { get; set; }
        public string VideoName { get; set; }
        public string ClassName { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string Status { get; set; }
        public int CriticalLevel { get; set; }
        public DateTime DateTimeDetection { get; set; }
    }
}
