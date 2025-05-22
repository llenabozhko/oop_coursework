using System;

namespace oop_coursework.Models
{
    public abstract class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Role { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        public string FullName => $"{FirstName} {LastName}";
    }
}
