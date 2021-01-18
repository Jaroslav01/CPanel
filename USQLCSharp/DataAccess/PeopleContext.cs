using Microsoft.EntityFrameworkCore;
using USQLCSharp.Models;

namespace USQLCSharp.DataAccess
{
    public class PeopleContext : DbContext
    {
        public PeopleContext(DbContextOptions options) : base(options) { }
        public DbSet<Person> People { get; set; }
        public DbSet<PersonState> PersonStates { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<Device> Devices { get; set; }

    }
}
