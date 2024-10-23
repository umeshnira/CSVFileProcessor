using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using FileProcessing.WebAPI.Controllers;
using FileProcessing.Domain.Interfaces;
using FileProcessing.Domain.Entities;


public class AuthControllerTests
{
    private readonly AuthController _authController;
    private readonly Mock<IUserService> _userServiceMock;
    private readonly Mock<ILogger<AuthController>> _loggerMock;

    public AuthControllerTests()
    {
        _userServiceMock = new Mock<IUserService>();
        _loggerMock = new Mock<ILogger<AuthController>>();

        _authController = new AuthController(_userServiceMock.Object, _loggerMock.Object);
    }

    // Test for Register action: Successful registration
    [Fact]
    public async Task Register_UserRegistersSuccessfully_ReturnsOk()
    {
        // Arrange
        var registerModel = new RegisterModel { Username = "testuser", Password = "password" };
        _userServiceMock.Setup(us => us.RegisterUser(registerModel)).ReturnsAsync(true);

        // Act
        var result = await _authController.Register(registerModel);

        // Xunit.Assert
        var okResult = Xunit.Assert.IsType<OkObjectResult>(result);
        Xunit.Assert.Equal("User registered successfully", okResult.Value);
        _loggerMock.Verify(l => l.LogInformation(It.IsAny<string>()), Times.Once);
    }

    // Test for Register action: User already exists (registration fails)
    [Fact]
    public async Task Register_UserAlreadyExists_ReturnsBadRequest()
    {
        // Arrange
        var registerModel = new RegisterModel { Username = "existinguser", Password = "password" };
        _userServiceMock.Setup(us => us.RegisterUser(registerModel)).ReturnsAsync(false);

        // Act
        var result = await _authController.Register(registerModel);

        // Xunit.Assert
        var badRequestResult = Xunit.Assert.IsType<BadRequestObjectResult>(result);
        Xunit.Assert.Equal("User already exists", badRequestResult.Value);
        _loggerMock.Verify(l => l.LogError(It.IsAny<string>()), Times.Never);
    }

    // Test for Register action: Exception handling
    [Fact]
    public async Task Register_ExceptionThrown_ReturnsBadRequestAndLogsError()
    {
        // Arrange
        var registerModel = new RegisterModel { Username = "testuser", Password = "password" };
        _userServiceMock.Setup(us => us.RegisterUser(registerModel)).ThrowsAsync(new System.Exception("Test exception"));

        // Act
        var result = await _authController.Register(registerModel);

        // Xunit.Assert
        var badRequestResult = Xunit.Assert.IsType<BadRequestObjectResult>(result);
        Xunit.Assert.Equal("User already exists", badRequestResult.Value);
        _loggerMock.Verify(l => l.LogError(It.IsAny<string>()), Times.Once);
    }

    // Test for Login action: Successful login
    [Fact]
    public void Login_ValidCredentials_ReturnsOk()
    {
        // Arrange
        var loginModel = new LoginModel { Username = "validuser", Password = "password" };
        _userServiceMock.Setup(us => us.Authenticate(loginModel)).ReturnsAsync("valid-token");

        // Act
        var result = _authController.Login(loginModel);

        // Xunit.Assert
        var okResult = Xunit.Assert.IsType<OkObjectResult>(result);
        Xunit.Assert.NotNull(okResult.Value);
        var token = ((dynamic)okResult.Value).Token;
        Xunit.Assert.Equal("valid-token", token);
    }

    // Test for Login action: Invalid credentials
    [Fact]
    public void Login_InvalidCredentials_ReturnsUnauthorized()
    {
        // Arrange
        var loginModel = new LoginModel { Username = "invaliduser", Password = "wrongpassword" };
        _userServiceMock.Setup(us => us.Authenticate(loginModel)).ReturnsAsync((string)null);

        // Act
        var result = _authController.Login(loginModel);

        // Xunit.Assert
        var unauthorizedResult = Xunit.Assert.IsType<UnauthorizedObjectResult>(result);
        Xunit.Assert.Equal("Invalid credentials", unauthorizedResult.Value);
    }
}
