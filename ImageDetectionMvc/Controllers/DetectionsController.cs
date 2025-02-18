using ImageDetectionMvc.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using System.Collections.Generic;

namespace ImageDetectionMvc.Controllers
{
    public class DetectionsController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiUrl;

        public DetectionsController(HttpClient httpClient, IOptions<ApiSettings> apiSettings)
        {
            _httpClient = httpClient;
            _apiUrl = apiSettings.Value.DetectionsApiUrl;
        }

        public async Task<IActionResult> Index()
        {
            var response = await _httpClient.GetAsync(_apiUrl);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                if (!string.IsNullOrEmpty(content))
                {
                    var detections = JsonSerializer.Deserialize<IEnumerable<Detection>>(content);
                    return View(detections);
                }
            }
            return View(new List<Detection>());
        }

        public async Task<IActionResult> Details(int id)
        {
            var response = await _httpClient.GetAsync($"{_apiUrl}/{id}");
            if (response.IsSuccessStatusCode)
            {
                var detection = await response.Content.ReadFromJsonAsync<Detection>();
                return View(detection);
            }
            return NotFound();
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Detection detection)
        {
            var json = JsonSerializer.Serialize(detection);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(_apiUrl, content);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }
            return View(detection);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var response = await _httpClient.GetAsync($"{_apiUrl}/{id}");
            if (response.IsSuccessStatusCode)
            {
                var detection = await response.Content.ReadFromJsonAsync<Detection>();
                return View(detection);
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, Detection detection)
        {
            var json = JsonSerializer.Serialize(detection);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"{_apiUrl}/{id}", content);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }
            return View(detection);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var response = await _httpClient.GetAsync($"{_apiUrl}/{id}");
            if (response.IsSuccessStatusCode)
            {
                var detection = await response.Content.ReadFromJsonAsync<Detection>();
                return View(detection);
            }
            return NotFound();
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var response = await _httpClient.DeleteAsync($"{_apiUrl}/{id}");
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }
            return View(new Detection { Id = id });
        }

        public async Task<IActionResult> ListImages(string videoName = null, string status = null, string className = null, int? criticalLevel = null)
        {
            var queryParams = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(videoName)) queryParams.Add("videoName", videoName);
            if (!string.IsNullOrEmpty(status)) queryParams.Add("status", status);
            if (!string.IsNullOrEmpty(className)) queryParams.Add("className", className);
            if (criticalLevel.HasValue) queryParams.Add("criticalLevel", criticalLevel.Value.ToString());

            var queryString = string.Join("&", queryParams.Select(kvp => $"{kvp.Key}={Uri.EscapeDataString(kvp.Value)}"));
            var requestUrl = $"{_apiUrl}?{queryString}";

            var response = await _httpClient.GetAsync(requestUrl);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                if (!string.IsNullOrEmpty(content))
                {
                    var detections = JsonSerializer.Deserialize<IEnumerable<Detection>>(content);
                    return View(detections);
                }
            }
            return View(new List<Detection>());
        }
    }
}
