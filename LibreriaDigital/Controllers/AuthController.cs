using LibreriaDigital.Models;
using LibreriaDigital.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Http;
using System.Web.Http;
using LibreriaDigital.Filters;

namespace LibreriaDigital.Controllers
{
    [RoutePrefix("api/auth")]
    public class AuthController : ApiController
    {
        private readonly IAuthService _authService;
        private readonly IJwtTokenService _jwtTokenService;

        // Constructor vacío (opcional, solo para producción)
        public AuthController()
            : this(new AuthService(), new JwtTokenService()) { }

        // Constructor que recibe servicios (para tests y DI)
        public AuthController(IAuthService authService, IJwtTokenService jwtTokenService)
        {
            _authService = authService;
            _jwtTokenService = jwtTokenService;
        }

        [HttpPost]
        [Route("register")]
        [ValidateModel]
        public IHttpActionResult Register([FromBody] RegisterRequest request)
        {
            if (_authService.UserExists(request.Email))
            {
                return BadRequest("El correo electrónico ya está en uso.");
            }

            var user = _authService.RegisterUser(request);
            return Ok(user);
        }

        [HttpPost]
        [Route("login")]
        [ValidateModel]
        [RateLimit]
        public IHttpActionResult Login([FromBody] LoginRequest request)
        {
            var token = _authService.AuthenticateUser(request);
            if (token == null)
            {
                return Unauthorized();
            }

            return Ok(new { Token = token });
        }
    }
}
