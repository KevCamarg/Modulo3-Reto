using System.Data.Entity.Migrations;
using System.Data.SQLite.EF6.Migrations;
using LibreriaDigital.Data;

internal sealed class Configuration : DbMigrationsConfiguration<ApplicationDbContext>
{
    public Configuration()
    {
        AutomaticMigrationsEnabled = true;
        SetSqlGenerator("System.Data.SQLite", new SQLiteMigrationSqlGenerator());
    }
}
