using LibreriaDigital.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LibreriaDigital.Data
{
    public static class SeedData
    {
        public static void Initialize(ApplicationDbContext context)
        {
            if (context.Users.Any() || context.Books.Any() || context.Loans.Any())
            {
                return; // La base de datos ya tiene datos
            }

            // Agregar usuarios iniciales
            context.Users.Add(new User { Name = "Admin", Email = "admin@example.com", Password = "Admin123" });
            context.Users.Add(new User { Name = "User1", Email = "user1@example.com", Password = "User123" });

            // Agregar libros iniciales
            context.Books.Add(new Book { Title = "C# Programming", Author = "John Doe", ISBN = "1234567890123", Quantity = 5 });
            context.Books.Add(new Book { Title = "ASP.NET MVC", Author = "Jane Smith", ISBN = "9876543210123", Quantity = 3 });

            context.SaveChanges();
        }
    }
}