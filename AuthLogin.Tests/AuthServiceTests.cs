using AuthLogin.Data;
using AuthLogin.Models;
using AuthLogin.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace AuthLogin.Tests
{
    public class AuthServiceTests
    {
        private ApplicationDbContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new ApplicationDbContext(options);
        }

        private IConfiguration GetConfiguration()
        {
            var config = new Dictionary<string, string>
            {
                {"Jwt:Key", "esta_es_una_clave_secreta_muy_larga_para_pruebas_12345"},
                {"Jwt:Issuer", "TestIssuer"},
                {"Jwt:Audience", "TestAudience"}
            };

            return new ConfigurationBuilder()
                .AddInMemoryCollection(config)
                .Build();
        }

        [Fact]
        public async Task LoginAsync_ShouldReturnSuccess_WhenCredentialsAreValid()
        {
            // Arrange
            var context = GetDbContext();
            var config = GetConfiguration();
            var service = new AuthService(context, config);

            var password = "password123";
            var user = new User
            {
                UserName = "testuser",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                Email = "test@test.com",
                FirstName = "Test",
                LastName = "User"
            };

            context.Users.Add(user);
            await context.SaveChangesAsync();

            var loginRequest = new LoginRequestDto
            {
                UserName = "testuser",
                Password = password
            };

            // Act
            var result = await service.LoginAsync(loginRequest);

            // Assert
            Assert.True(result.success);
            Assert.Equal("testuser", user.UserName);
            Assert.NotEmpty(result.token);
        }

        [Fact]
        public async Task LoginAsync_ShouldReturnFailure_WhenUserDoesNotExist()
        {
            // Arrange
            var context = GetDbContext();
            var config = GetConfiguration();
            var service = new AuthService(context, config);

            var loginRequest = new LoginRequestDto
            {
                UserName = "nonexistent",
                Password = "password123"
            };

            // Act
            var result = await service.LoginAsync(loginRequest);

            // Assert
            Assert.False(result.success);
            Assert.Equal("Usuario o contraseña incorrecto", result.message);
        }
    }
}
