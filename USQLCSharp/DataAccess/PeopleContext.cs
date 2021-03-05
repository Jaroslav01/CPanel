using Microsoft.EntityFrameworkCore;
using USQLCSharp.Models;

namespace USQLCSharp.DataAccess
{
    public class PeopleContext : DbContext
    {
        public PeopleContext(DbContextOptions options) : base(options) { }
        public PeopleContext() : base() { }
        public DbSet<Person> People { get; set; }
        public DbSet<PersonState> PersonStates { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<Device> Devices { get; set; }
        public DbSet<Parameter> Parameters { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=DESKTOP-611SA6B;Database=CPanel2d;Trusted_Connection=True;");
            //optionsBuilder.UseSqlServer("Server=LAPTOP-NL8GVBDL\\SQLEXPRESS;Database=CPanel;Trusted_Connection=True;");
        }
    }
}
