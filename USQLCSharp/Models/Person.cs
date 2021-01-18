using System.Collections.Generic;

namespace USQLCSharp.Models
{
    public class Person
    {
        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Password { get; set; }

        public string Birthsday { get; set; }

        public List<PersonState> PersonStates { get; set; } = new List<PersonState>();

        public List<Contact> Contacts { get; set; } = new List<Contact>();

        public List<Device> Devices { get; set; } = new List<Device>();
    }
}
