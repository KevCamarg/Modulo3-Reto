using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BCrypt.Net;

namespace LibreriaDigital.Services
{
    public class PasswordHasher
    {
        private const int WorkFactor = 12;

        /// <summary>
        /// Hashea una contraseña utilizando BCrypt.
        /// </summary>
        /// <param name="password">La contraseña a hashear.</param>
        /// <returns>El hash de la contraseña.</returns>
        /// <exception cref="ArgumentNullException">Lanza una excepción si la contraseña es nula.</exception>
        public string HashPassword(string password)
        {
            if (password == null)
            {
                throw new ArgumentNullException(nameof(password), "La contraseña no puede ser nula.");
            }

            return BCrypt.Net.BCrypt.HashPassword(password, WorkFactor);
        }

        /// <summary>
        /// Verifica si una contraseña coincide con su hash.
        /// </summary>
        /// <param name="password">La contraseña a verificar.</param>
        /// <param name="hash">El hash de la contraseña.</param>
        /// <returns>True si la contraseña coincide con el hash, de lo contrario false.</returns>
        /// <exception cref="ArgumentNullException">Lanza una excepción si la contraseña o el hash son nulos.</exception>
        public bool VerifyPassword(string password, string hash)
        {
            if (password == null)
            {
                throw new ArgumentNullException(nameof(password), "La contraseña no puede ser nula.");
            }

            if (hash == null)
            {
                throw new ArgumentNullException(nameof(hash), "El hash no puede ser nulo.");
            }

            return BCrypt.Net.BCrypt.Verify(password, hash);
        }
    }
}