using LibreriaDigital.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using static System.Data.Entity.Migrations.Model.UpdateDatabaseOperation;

namespace LibreriaDigital.Data
{
	public class ApplicationDbContext : DbContext
	{
        public ApplicationDbContext()
            : base("DefaultConnection") // Debe coincidir con tu Web.config
        {
            // Inicializador: crea la DB si no existe
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<ApplicationDbContext, Configuration>());

        }

        public DbSet<User> Users { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<Loan> Loans { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}