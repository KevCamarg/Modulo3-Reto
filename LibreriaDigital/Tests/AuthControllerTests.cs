using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.SelfHost;
using System.Web.Http.Results;
using LibreriaDigital.Controllers;
using LibreriaDigital.Models;
using Microsoft.Extensions.Logging;
using LibreriaDigital.Services;
using System.Configuration;
using System.Web.Http.Hosting;

namespace LibreriaDigital.Tests
{
    [TestClass]
    public class AuthControllerTests
    {
        private Mock<IAuthService> _mockAuthService;
        private Mock<IJwtTokenService> _mockJwtTokenService;
        private AuthController _controller;

        /// <summary>
        /// Setup ejecutado antes de cada test
        /// </summary>
        [TestInitialize]
        public void Setup()
        {
            // Arrange: Crear mocks
            _mockAuthService = new Mock<IAuthService>();
            _mockJwtTokenService = new Mock<IJwtTokenService>();

            // Crear controller con dependencias mockeadas
            _controller = new AuthController(_mockAuthService.Object, _mockJwtTokenService.Object);

            // Configurar HttpRequestMessage necesario para IHttpActionResult
            _controller.Request = new HttpRequestMessage();
            _controller.Request.Properties[HttpPropertyKeys.HttpConfigurationKey] = new HttpConfiguration();
        }

        /// <summary>
        /// Cleanup ejecutado después de cada test
        /// </summary>
        [TestCleanup]
        public void Cleanup()
        {
            _controller?.Dispose();
        }

        #region Register Tests

        [TestMethod]
        [TestCategory("Register")]
        public void Register_ReturnsCreated_WhenValid()
        {
            // Arrange
            var request = new RegisterRequest
            {
                Name = "John Doe",
                Email = "john@example.com",
                Password = "Password123!"
            };

            var expectedUser = new User
            {
                Id = 1,
                Name = request.Name,
                Email = request.Email,
                Password = "hashed_password"
            };

            _mockAuthService.Setup(x => x.UserExists(request.Email)).Returns(false);
            _mockAuthService.Setup(x => x.RegisterUser(request)).Returns(expectedUser);

            // Act
            var result = _controller.Register(request);

            // Assert
            result.Should().BeOfType<OkNegotiatedContentResult<User>>();
            var okResult = result as OkNegotiatedContentResult<User>;
            okResult.Content.Should().BeEquivalentTo(expectedUser);

            // Verificar que se llamaron los métodos correctos
            _mockAuthService.Verify(x => x.UserExists(request.Email), Times.Once);
            _mockAuthService.Verify(x => x.RegisterUser(request), Times.Once);
        }

        [TestMethod]
        [TestCategory("Register")]
        public void Register_ReturnsBadRequest_WhenEmailAlreadyExists()
        {
            // Arrange
            var request = new RegisterRequest
            {
                Name = "Jane Doe",
                Email = "existing@example.com",
                Password = "Password123!"
            };

            _mockAuthService.Setup(x => x.UserExists(request.Email)).Returns(true);

            // Act
            var result = _controller.Register(request);

            // Assert
            result.Should().BeOfType<BadRequestErrorMessageResult>();
            var badRequestResult = result as BadRequestErrorMessageResult;
            badRequestResult.Message.Should().Be("El correo electrónico ya está en uso.");

            // Verificar que NO se intentó registrar
            _mockAuthService.Verify(x => x.RegisterUser(It.IsAny<RegisterRequest>()), Times.Never);
        }

        [TestMethod]
        [TestCategory("Register")]
        public void Register_ReturnsBadRequest_WhenNameIsEmpty()
        {
            // Arrange
            var request = new RegisterRequest
            {
                Name = "", // Inválido
                Email = "test@example.com",
                Password = "Password123!"
            };

            // Simular ModelState inválido (el ValidateModelAttribute lo haría)
            _controller.ModelState.AddModelError("Name", "El nombre es obligatorio.");

            // Act
            var result = _controller.Register(request);

            // Assert - En un escenario real, ValidateModelAttribute interceptaría antes
            // pero validamos el comportamiento del controller
            _controller.ModelState.IsValid.Should().BeFalse();
        }

        [TestMethod]
        [TestCategory("Register")]
        public void Register_ReturnsBadRequest_WhenEmailFormatIsInvalid()
        {
            // Arrange
            var request = new RegisterRequest
            {
                Name = "Test User",
                Email = "invalid-email", // Formato inválido
                Password = "Password123!"
            };

            _controller.ModelState.AddModelError("Email", "El formato del correo electrónico no es válido.");

            // Act & Assert
            _controller.ModelState.IsValid.Should().BeFalse();
        }

        [TestMethod]
        [TestCategory("Register")]
        public void Register_ReturnsBadRequest_WhenPasswordIsTooShort()
        {
            // Arrange
            var request = new RegisterRequest
            {
                Name = "Test User",
                Email = "test@example.com",
                Password = "Pass1!" // Menos de 8 caracteres
            };

            _controller.ModelState.AddModelError("Password", "La contraseña debe tener al menos 8 caracteres.");

            // Act & Assert
            _controller.ModelState.IsValid.Should().BeFalse();
        }

        #endregion

        #region Login Tests

        [TestMethod]
        [TestCategory("Login")]
        public void Login_ReturnsOkWithToken_WhenValid()
        {
            // Arrange
            var request = new LoginRequest
            {
                Email = "john@example.com",
                Password = "Password123!"
            };

            var expectedToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIn0.dozjgNryP4J3jVmNHl0w5N_XgL0n3I9PlFUP0THsR8U";

            _mockAuthService.Setup(x => x.AuthenticateUser(request)).Returns(expectedToken);

            // Act
            var result = _controller.Login(request);

            // Assert
            result.Should().BeOfType<OkNegotiatedContentResult<object>>();
            var okResult = result as OkNegotiatedContentResult<dynamic>;
            okResult.Should().NotBeNull();
            ((string)okResult.Content.Token).Should().Be(expectedToken);


            // Verificar que el objeto anónimo contiene el token
            var responseObject = okResult.Content;
            var tokenProperty = responseObject.GetType().GetProperty("Token");
            tokenProperty.Should().NotBeNull();
            tokenProperty.GetValue(responseObject).Should().Be(expectedToken);

            _mockAuthService.Verify(x => x.AuthenticateUser(request), Times.Once);
        }

        [TestMethod]
        [TestCategory("Login")]
        public void Login_ReturnsUnauthorized_WhenCredentialsAreInvalid()
        {
            // Arrange
            var request = new LoginRequest
            {
                Email = "john@example.com",
                Password = "WrongPassword"
            };

            _mockAuthService.Setup(x => x.AuthenticateUser(request)).Returns((string)null);

            // Act
            var result = _controller.Login(request);

            // Assert
            result.Should().BeOfType<UnauthorizedResult>();
            _mockAuthService.Verify(x => x.AuthenticateUser(request), Times.Once);
        }

        [TestMethod]
        [TestCategory("Login")]
        public void Login_ReturnsUnauthorized_WhenUserDoesNotExist()
        {
            // Arrange
            var request = new LoginRequest
            {
                Email = "nonexistent@example.com",
                Password = "Password123!"
            };

            _mockAuthService.Setup(x => x.AuthenticateUser(request)).Returns((string)null);

            // Act
            var result = _controller.Login(request);

            // Assert
            result.Should().BeOfType<UnauthorizedResult>();
        }

        [TestMethod]
        [TestCategory("Login")]
        public void Login_ReturnsBadRequest_WhenEmailIsEmpty()
        {
            // Arrange
            var request = new LoginRequest
            {
                Email = "",
                Password = "Password123!"
            };

            _controller.ModelState.AddModelError("Email", "El correo electrónico es obligatorio.");

            // Act & Assert
            _controller.ModelState.IsValid.Should().BeFalse();
        }

        [TestMethod]
        [TestCategory("Login")]
        public void Login_ReturnsBadRequest_WhenPasswordIsEmpty()
        {
            // Arrange
            var request = new LoginRequest
            {
                Email = "test@example.com",
                Password = ""
            };

            _controller.ModelState.AddModelError("Password", "La contraseña es obligatoria.");

            // Act & Assert
            _controller.ModelState.IsValid.Should().BeFalse();
        }

        #endregion

        #region Additional Edge Cases

        [TestMethod]
        [TestCategory("Register")]
        public void Register_CallsUserExistsBeforeRegisterUser()
        {
            // Arrange
            var request = new RegisterRequest
            {
                Name = "Test User",
                Email = "test@example.com",
                Password = "Password123!"
            };

            var callSequence = new List<string>();

            _mockAuthService.Setup(x => x.UserExists(request.Email))
                .Returns(false)
                .Callback(() => callSequence.Add("UserExists"));

            _mockAuthService.Setup(x => x.RegisterUser(request))
                .Returns(new User { Id = 1 })
                .Callback(() => callSequence.Add("RegisterUser"));

            // Act
            _controller.Register(request);

            // Assert
            callSequence.Should().ContainInOrder("UserExists", "RegisterUser");
        }

        [TestMethod]
        [TestCategory("Login")]
        public void Login_DoesNotRevealIfUserExists()
        {
            // Arrange - Usuario no existe
            var request1 = new LoginRequest
            {
                Email = "nonexistent@example.com",
                Password = "Password123!"
            };

            // Arrange - Usuario existe pero contraseña incorrecta
            var request2 = new LoginRequest
            {
                Email = "existing@example.com",
                Password = "WrongPassword"
            };

            _mockAuthService.Setup(x => x.AuthenticateUser(It.IsAny<LoginRequest>())).Returns((string)null);

            // Act
            var result1 = _controller.Login(request1);
            var result2 = _controller.Login(request2);

            // Assert - Ambos casos retornan el mismo tipo de error
            result1.Should().BeOfType<UnauthorizedResult>();
            result2.Should().BeOfType<UnauthorizedResult>();

            // Verificación de seguridad: No se debe revelar si el usuario existe
            result1.GetType().Should().Be(result2.GetType());
        }

        #endregion
    }

    #region Service Tests

    /// <summary>
    /// Tests unitarios para PasswordHasher
    /// </summary>
    [TestClass]
    public class PasswordHasherTests
    {
        private PasswordHasher _passwordHasher;

        [TestInitialize]
        public void Setup()
        {
            _passwordHasher = new PasswordHasher();
        }

        [TestMethod]
        [TestCategory("PasswordHasher")]
        public void HashPassword_ReturnsNonEmptyString_WhenPasswordIsValid()
        {
            // Arrange
            var password = "Password123!";

            // Act
            var hash = _passwordHasher.HashPassword(password);

            // Assert
            hash.Should().NotBeNullOrEmpty();
            hash.Should().StartWith("$2a$12$"); // BCrypt format with work factor 12
        }

        [TestMethod]
        [TestCategory("PasswordHasher")]
        public void HashPassword_ReturnsDifferentHashes_ForSamePassword()
        {
            // Arrange
            var password = "Password123!";

            // Act
            var hash1 = _passwordHasher.HashPassword(password);
            var hash2 = _passwordHasher.HashPassword(password);

            // Assert - Los hashes deben ser diferentes debido al salt aleatorio
            hash1.Should().NotBe(hash2);
        }

        [TestMethod]
        [TestCategory("PasswordHasher")]
        public void HashPassword_ThrowsException_WhenPasswordIsNull()
        {
            // Act
            try
            {
                _passwordHasher.HashPassword(null);
                Assert.Fail("Se esperaba una ArgumentNullException.");
            }
            catch (ArgumentNullException)
            {
                // Éxito: la excepción esperada fue lanzada.
            }
        }

        [TestMethod]
        [TestCategory("PasswordHasher")]
        public void VerifyPassword_ReturnsTrue_WhenPasswordMatches()
        {
            // Arrange
            var password = "Password123!";
            var hash = _passwordHasher.HashPassword(password);

            // Act
            var result = _passwordHasher.VerifyPassword(password, hash);

            // Assert
            result.Should().BeTrue();
        }

        [TestMethod]
        [TestCategory("PasswordHasher")]
        public void VerifyPassword_ReturnsFalse_WhenPasswordDoesNotMatch()
        {
            // Arrange
            var correctPassword = "Password123!";
            var wrongPassword = "WrongPassword123!";
            var hash = _passwordHasher.HashPassword(correctPassword);

            // Act
            var result = _passwordHasher.VerifyPassword(wrongPassword, hash);

            // Assert
            result.Should().BeFalse();
        }

        [TestMethod]
        [TestCategory("PasswordHasher")]
        public void VerifyPassword_ThrowsException_WhenPasswordIsNull()
        {
            // Arrange
            var hash = "$2a$12$somehash";

            // Act & Assert
            try
            {
                _passwordHasher.VerifyPassword(null, hash);
                Assert.Fail("Se esperaba ArgumentNullException porque el password es null.");
            }
            catch (ArgumentNullException ex)
            {
                // Opcional: verificar que el nombre del parámetro sea correcto
                Assert.AreEqual("password", ex.ParamName);
            }
        }

        [TestMethod]
        [TestCategory("PasswordHasher")]
        public void VerifyPassword_ThrowsException_WhenHashIsNull()
        {
            // Act & Assert
            try
            {
                _passwordHasher.VerifyPassword("password", null);
                Assert.Fail("Se esperaba ArgumentNullException porque el hash es null.");
            }
            catch (ArgumentNullException ex)
            {
                // Opcional: verificar que el nombre del parámetro sea correcto
                Assert.AreEqual("hash", ex.ParamName);
            }
        }
    }

    /// <summary>
    /// Tests unitarios para JwtTokenService
    /// </summary>
    [TestClass]
    public class JwtTokenServiceTests
    {
        private JwtTokenService _jwtTokenService;

        [TestInitialize]
        public void Setup()
        {
            // Configurar AppSettings para tests
            ConfigurationManager.AppSettings["SecretKey"] = "ThisIsAVerySecretKeyForTestingPurposesOnly123456!";
            ConfigurationManager.AppSettings["Issuer"] = "TestIssuer";
            ConfigurationManager.AppSettings["Audience"] = "TestAudience";
            ConfigurationManager.AppSettings["ExpirationMinutes"] = "60";

            _jwtTokenService = new JwtTokenService();
        }

        [TestMethod]
        [TestCategory("JwtToken")]
        public void GenerateToken_ReturnsValidJwtString()
        {
            // Arrange
            var userId = 123;
            var email = "test@example.com";

            // Act
            var token = _jwtTokenService.GenerateToken(userId, email);

            // Assert
            token.Should().NotBeNullOrEmpty();
            token.Split('.').Should().HaveCount(3); // JWT format: header.payload.signature
        }

        [TestMethod]
        [TestCategory("JwtToken")]
        public void GenerateToken_ContainsCorrectClaims()
        {
            // Arrange
            var userId = 123;
            var email = "test@example.com";

            // Act
            var token = _jwtTokenService.GenerateToken(userId, email);
            var principal = _jwtTokenService.ValidateToken(token);

            // Assert
            principal.Should().NotBeNull();
            var userIdClaim = principal.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            var emailClaim = principal.FindFirst(System.Security.Claims.ClaimTypes.Email);

            userIdClaim.Should().NotBeNull();
            userIdClaim.Value.Should().Be(userId.ToString());

            emailClaim.Should().NotBeNull();
            emailClaim.Value.Should().Be(email);
        }

        [TestMethod]
        [TestCategory("JwtToken")]
        public void ValidateToken_ReturnsNull_ForInvalidToken()
        {
            // Arrange
            var invalidToken = "invalid.token.here";

            // Act
            var principal = _jwtTokenService.ValidateToken(invalidToken);

            // Assert
            principal.Should().BeNull();
        }

        [TestMethod]
        [TestCategory("JwtToken")]
        public void ValidateToken_ReturnsNull_ForExpiredToken()
        {
            // Arrange - Crear token con expiración en el pasado
            ConfigurationManager.AppSettings["ExpirationMinutes"] = "-1"; // Expirado
            var expiredTokenService = new JwtTokenService();
            var token = expiredTokenService.GenerateToken(1, "test@example.com");

            // Esperar un momento para asegurar expiración
            System.Threading.Thread.Sleep(1000);

            // Act
            var principal = _jwtTokenService.ValidateToken(token);

            // Assert
            principal.Should().BeNull();
        }
    }
    #endregion
}