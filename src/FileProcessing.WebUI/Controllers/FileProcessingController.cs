using FileProcessing.Domain.DTO;
using FileProcessing.Domain.Entities;
using FileProcessing.WebUI.Helper;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Text.Json;

namespace FileProcessing.WebUI.Controllers
{
    public class FileProcessingController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly ILogger<FileProcessingController> _logger;

        public FileProcessingController(IHttpClientFactory httpClientFactory, IConfiguration configuration, ILogger<FileProcessingController> logger)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [ViewLayout("_Layout")]
        [HttpGet]
        public IActionResult UploadFile()
        {
            return View();
        }

        [ViewLayout("_Layout")]
        [HttpPost]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                ModelState.AddModelError(string.Empty, "Please select a file.");
                return View();
            }

            var allowedExtensions = new[] { ".csv" };
            var fileExtension = Path.GetExtension(file.FileName).ToLower();

            if (!allowedExtensions.Contains(fileExtension))
            {
                ModelState.AddModelError(string.Empty, "Invalid file type. Only CSV files are allowed.");
                ViewBag.Message = "Invalid file type. Only CSV files are allowed.";
                return View();
            }

            var uploadPath = await UploadCSVFile(file);
            var token = HttpContext.Session.GetString("JWToken");

            if (string.IsNullOrEmpty(token))
            {
                _logger.LogWarning("Attempt to upload file without authentication.");
                ModelState.AddModelError(string.Empty, "User is not authenticated.");
                return View();
            }

            var response = await TriggerFileProcessingAsync(uploadPath, token);

            if (response.IsSuccessStatusCode)
            {
                ViewBag.Message = "File uploaded and processing started successfully.";
            }
            else
            {
                _logger.LogError("Error occurred during file upload for {FileName}: {Error}", file.FileName, response.ReasonPhrase);
                ModelState.AddModelError(string.Empty, "Error starting file processing.");
            }

            return View();
        }

        private async Task<HttpResponseMessage> TriggerFileProcessingAsync(string uploadPath, string token)
        {
            var client = _httpClientFactory.CreateClient();
            var apiUrl = $"{_configuration["ApiUrl"]}api/files/process";
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var content = new StringContent(JsonSerializer.Serialize(new { FilePath = uploadPath }), Encoding.UTF8, "application/json");
            return await client.PostAsync(apiUrl, content);
        }

        private static async Task<string> UploadCSVFile(IFormFile file)
        {
            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "UploadedFiles");

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            var uploadPath = Path.Combine(folderPath, Path.GetFileName(file.FileName));
            await using (var stream = new FileStream(uploadPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return uploadPath;
        }

        [ViewLayout("_Layout")]
        [HttpGet]
        public async Task<IActionResult> Status()
        {
            var client = _httpClientFactory.CreateClient();
            var apiUrl = $"{_configuration["ApiUrl"]}api/files/status";

            var response = await client.GetAsync(apiUrl);
            if (response.IsSuccessStatusCode)
            {
                var fileStatusList = await response.Content.ReadFromJsonAsync<List<FileProcessingRecordDTO>>();
                if (fileStatusList != null)
                {
                    return View(fileStatusList);
                }
                _logger.LogError("Failed to deserialize file processing status response.");
            }
            else
            {
                _logger.LogError("Failed to fetch file status from API. Status code: {StatusCode}", response.StatusCode);
            }

            return View(new List<FileProcessingRecordDTO>());
        }
    }
}
