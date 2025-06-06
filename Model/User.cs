using System;

namespace oop_coursework.Models
{
    public abstract class User
    {
        public int Id { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public required string Role { get; set; }
        public required string Username { get; set; }
        public required string Password { get; set; }

        public string FullName => $"{FirstName} {LastName}";
    }
}
