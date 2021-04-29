using Microsoft.EntityFrameworkCore;
using USQLCSharp.Models;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using System;

namespace USQLCSharp.DataAccess
{
    public class PeopleContext : DbContext
    {
        public PeopleContext(DbContextOptions options) : base(options) { }
        public PeopleContext() : base() { }
        public DbSet<Person> Person { get; set; }
        public DbSet<PersonState> PersonStates { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<Device> Devices { get; set; }
        public DbSet<Parameter> Parameters { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //optionsBuilder.UseSqlServer("Server=DESKTOP-611SA6B;Database=CPanel2d;Trusted_Connection=True;");
            //optionsBuilder.UseSqlServer("Server=LAPTOP-NL8GVBDL\\SQLEXPRESS;Database=CPanel;Trusted_Connection=True;");
            optionsBuilder.UseMySql("server=176.36.127.144;user=CPanel;password=220977qQ;database=CPanel",
                    mysqlOptions =>
                    {
                        mysqlOptions.ServerVersion(new Version(10, 3, 27), ServerType.MySql);
                    });
        }
    }
}
