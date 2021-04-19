namespace USQLCSharp.Models
{
    public class Contact
    {
        public int Id { get; set; }

        public string PhoneNumber { get; set; }

        public string EmailAddress { get; set; }

        public bool SubmitPhoneNumber { get; set; }

        public bool SubmitEmailAddress { get; set; }

        public int Admittance { get; set; }
    }
}
