using FileProcessing.Application.Services;
using FileProcessing.Domain.Entities;
using FileProcessing.Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FileProcessing.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IUserService userService, ILogger<AuthController> logger)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for user registration: {Username}", model.Username);
                return BadRequest("Invalid model");
            }

            try
            {
                if (await _userService.RegisterUser(model))
                {
                    _logger.LogInformation("User {Username} registered successfully.", model.Username);
                    return Ok("User registered successfully");
                }
                return Conflict("User already exists");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during registration for user {Username}.", model.Username);
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for login: {Username}", model.Username);
                return BadRequest("Invalid model");
            }

            try
            {
                var token = await _userService.Authenticate(model);
                if (token == null)
                {
                    _logger.LogWarning("Failed login attempt for user {Username}.", model.Username);
                    return Unauthorized("Invalid credentials");
                }

                _logger.LogInformation("User {Username} logged in successfully.", model.Username);
                return Ok(new { Token = token });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during login for user {Username}.", model.Username);
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
            }
        }
    }
}
