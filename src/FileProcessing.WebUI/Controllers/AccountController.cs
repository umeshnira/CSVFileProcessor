using FileProcessing.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text;
using Newtonsoft.Json.Linq;
using FileProcessing.WebUI.Helper;
using Microsoft.Extensions.Logging;

namespace FileProcessing.WebUI.Controllers
{
    public class AccountController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AccountController> _logger;
        private const string JsonMediaType = "application/json";

        public AccountController(IHttpClientFactory httpClientFactory, IConfiguration configuration, ILogger<AccountController> logger)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _logger = logger;
        }

        [ViewLayout("_AccountLayout")]
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var client = _httpClientFactory.CreateClient();
                var apiUrl = $"{_configuration["ApiUrl"]}api/auth/register";

                var content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, JsonMediaType);
                var response = await client.PostAsync(apiUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("User registration successful for {Username}", model.Username);
                    return RedirectToAction("Login");
                }

                _logger.LogWarning("Registration failed with status code {StatusCode}", response.StatusCode);
                ModelState.AddModelError(string.Empty, "Registration failed");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during user registration for {Username}", model.Username);
                ModelState.AddModelError(string.Empty, "An unexpected error occurred.");
            }

            return View(model);
        }

        [ViewLayout("_AccountLayout")]
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var client = _httpClientFactory.CreateClient();
                var apiUrl = $"{_configuration["ApiUrl"]}api/auth/login";

                var content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, JsonMediaType);
                var response = await client.PostAsync(apiUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    dynamic data = JObject.Parse(jsonResponse);
                    var token = data?.token;

                    if (token == null)
                    {
                        _logger.LogError("Token extraction failed for user {Username}", model.Username);
                        ModelState.AddModelError(string.Empty, "Token extraction failed");
                        return View(model);
                    }

                    _logger.LogInformation("User {Username} logged in successfully", model.Username);

                    // Store token securely in session or cookies
                    HttpContext.Session.SetString("JWToken", (string)token);
                    return RedirectToAction("Status", "FileProcessing");
                }

                _logger.LogWarning("Login failed for user {Username}", model.Username);
                ModelState.AddModelError(string.Empty, "Login failed");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during login for {Username}", model.Username);
                ModelState.AddModelError(string.Empty, "An unexpected error occurred.");
            }

            return View(model);
        }
    }
}
