using ImageDetectionMvc.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

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
            var detections = await response.Content.ReadFromJsonAsync<IEnumerable<Detection>>();
            return View(detections);
        }

        public async Task<IActionResult> Details(int id)
        {
            var response = await _httpClient.GetAsync($"{_apiUrl}/{id}");
            var detection = await response.Content.ReadFromJsonAsync<Detection>();
            return View(detection);
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
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var response = await _httpClient.GetAsync($"{_apiUrl}/{id}");
            var detection = await response.Content.ReadFromJsonAsync<Detection>();
            return View(detection);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, Detection detection)
        {
            var json = JsonSerializer.Serialize(detection);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"{_apiUrl}/{id}", content);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var response = await _httpClient.GetAsync($"{_apiUrl}/{id}");
            var detection = await response.Content.ReadFromJsonAsync<Detection>();
            return View(detection);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var response = await _httpClient.DeleteAsync($"{_apiUrl}/{id}");
            return RedirectToAction(nameof(Index));
        }
    }
}
