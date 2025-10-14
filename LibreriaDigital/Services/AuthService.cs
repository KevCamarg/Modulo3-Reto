using LibreriaDigital.Data;
using LibreriaDigital.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace LibreriaDigital.Services
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly PasswordHasher _passwordHasher;
        private readonly JwtTokenService _jwtTokenService;

        public AuthService()
        {
            _context = new ApplicationDbContext();
            _passwordHasher = new PasswordHasher();
            _jwtTokenService = new JwtTokenService();
        }

        public bool UserExists(string email)
        {
            return _context.Users.Any(u => u.Email == email);
        }

        public User RegisterUser(RegisterRequest request)
        {
            var user = new User
            {
                Name = request.Name,
                Email = request.Email,
                Password = _passwordHasher.HashPassword(request.Password) // Usar PasswordHasher
            };

            _context.Users.Add(user);
            _context.SaveChanges();
            return user;
        }

        public string AuthenticateUser(LoginRequest request)
        {
            var user = _context.Users.SingleOrDefault(u => u.Email == request.Email);
            if (user == null || !_passwordHasher.VerifyPassword(request.Password, user.Password)) // Usar PasswordHasher
            {
                return null; // Autenticación fallida
            }

            return _jwtTokenService.GenerateToken(user.Id, user.Email); // Implementar generación de JWT
        }

        public User GetUserById(int userId)
        {
            return _context.Users.Find(userId);
        }
    }
}