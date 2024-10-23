using FileProcessing.WebUI.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using FileProcessing.Domain.Entities;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
namespace FileProcessing.UnitTest
{
    public class AccountControllerTests
    {
        private readonly Mock<IHttpClientFactory> _httpClientFactoryMock;
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly AccountController _accountController;

        public AccountControllerTests()
        {
            _httpClientFactoryMock = new Mock<IHttpClientFactory>();
            _configurationMock = new Mock<IConfiguration>();
            _accountController = new AccountController(_httpClientFactoryMock.Object, _configurationMock.Object);
        }

        [Fact]
        public async Task Register_ReturnsRedirectToAction_WhenRegistrationSuccessful()
        {
            // Arrange
            var model = new RegisterModel { Username = "testuser", Password = "Test@123", Email = "testuser@example.com" };
            var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK);

            var mockHttpClient = new Mock<HttpClient>();
            _httpClientFactoryMock.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(mockHttpClient.Object);

            var mockClientHandler = new Mock<HttpMessageHandler>();
            mockHttpClient.Setup(client => client.PostAsync(It.IsAny<string>(), It.IsAny<HttpContent>()))
                          .ReturnsAsync(httpResponseMessage);

            _configurationMock.Setup(config => config["ApiUrl"]).Returns("http://mockapiurl.com");

            // Act
            var result = await _accountController.Register(model) as RedirectToActionResult;

            // Xunit.Assert
            Xunit.Assert.NotNull(result);
            Xunit.Assert.Equal("Login", result.ActionName);
        }

        [Fact]
        public async Task Register_ReturnsView_WhenRegistrationFails()
        {
            // Arrange
            var model = new RegisterModel { Username = "testuser", Password = "Test@123", Email = "testuser@example.com" };
            var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.BadRequest);

            var mockHttpClient = new Mock<HttpClient>();
            _httpClientFactoryMock.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(mockHttpClient.Object);

            mockHttpClient.Setup(client => client.PostAsync(It.IsAny<string>(), It.IsAny<HttpContent>()))
                          .ReturnsAsync(httpResponseMessage);

            _configurationMock.Setup(config => config["ApiUrl"]).Returns("http://mockapiurl.com");

            // Act
            var result = await _accountController.Register(model) as ViewResult;

            // Xunit.Assert
            Xunit.Assert.NotNull(result);
            Xunit.Assert.False(_accountController.ModelState.IsValid);
        }

        [Fact]
        public async Task Login_ReturnsRedirectToAction_WhenLoginSuccessful()
        {
            // Arrange
            var model = new LoginModel { Username = "testuser", Password = "Test@123" };
            var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonSerializer.Serialize(new { token = "mocktoken" }), Encoding.UTF8, "application/json")
            };

            var mockHttpClient = new Mock<HttpClient>();
            _httpClientFactoryMock.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(mockHttpClient.Object);

            mockHttpClient.Setup(client => client.PostAsync(It.IsAny<string>(), It.IsAny<HttpContent>()))
                          .ReturnsAsync(httpResponseMessage);

            _configurationMock.Setup(config => config["ApiUrl"]).Returns("http://mockapiurl.com");
            
            // Act
            var result = await _accountController.Login(model) as RedirectToActionResult;

            // Xunit.Assert
            Xunit.Assert.NotNull(result);
            Xunit.Assert.Equal("FileProcessing", result.ActionName);
            Xunit.Assert.Equal("Status", result.ControllerName);            
        }

        [Fact]
        public async Task Login_ReturnsView_WhenLoginFails()
        {
            // Arrange
            var model = new LoginModel { Username = "testuser", Password = "Test@123" };
            var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.Unauthorized);

            var mockHttpClient = new Mock<HttpClient>();
            _httpClientFactoryMock.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(mockHttpClient.Object);

            mockHttpClient.Setup(client => client.PostAsync(It.IsAny<string>(), It.IsAny<HttpContent>()))
                          .ReturnsAsync(httpResponseMessage);

            _configurationMock.Setup(config => config["ApiUrl"]).Returns("http://mockapiurl.com");

            // Act
            var result = await _accountController.Login(model) as ViewResult;

            // Xunit.Assert
            Xunit.Assert.NotNull(result);
            Xunit.Assert.False(_accountController.ModelState.IsValid);
        }
    }
}
