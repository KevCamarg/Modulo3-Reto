using LibreriaDigital.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;

namespace LibreriaDigital.Services
{
    /// <summary>
    /// Interfaz para AuthService - permite mockear en tests
    /// </summary>
    public interface IAuthService
    {
        bool UserExists(string email);
        User RegisterUser(RegisterRequest request);
        string AuthenticateUser(LoginRequest request);
        User GetUserById(int userId);
    }

    /// <summary>
    /// Interfaz para JwtTokenService - permite mockear en tests
    /// </summary>
    public interface IJwtTokenService
    {
        string GenerateToken(int userId, string email);
        ClaimsPrincipal ValidateToken(string token);
    }
}