using System;

namespace oop_coursework.Models
{
    public abstract class User
    {
        private int _id;
        private string _firstName = string.Empty;
        private string _lastName = string.Empty;
        private DateTime _dateOfBirth;
        private string _role = string.Empty;
        private string _username = string.Empty;
        private string _password = string.Empty;

        public int Id
        {
            get => _id;
            set
            {
                if (value <= 0)
                    throw new ArgumentException("Id must be positive");
                _id = value;
            }
        }

        public string FirstName
        {
            get => _firstName;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("First name cannot be empty");
                _firstName = value.Trim();
            }
        }

        public string LastName
        {
            get => _lastName;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Last name cannot be empty");
                _lastName = value.Trim();
            }
        }

        public DateTime DateOfBirth
        {
            get => _dateOfBirth;
            set
            {
                if (value > DateTime.Now)
                    throw new ArgumentException("Date of birth cannot be in the future");
                if (value.Year < 0)
                    throw new ArgumentException("Invalid date of birth year");
                _dateOfBirth = value;
            }
        }

        public string Role
        {
            get => _role;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Role cannot be empty");
                if (value != "Student" && value != "Teacher" && value != "Administrator")
                    throw new ArgumentException("Invalid role");
                _role = value;
            }
        }

        public string Username
        {
            get => _username;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Username cannot be empty");
                if (value.Length < 2)
                    throw new ArgumentException("Username must be at least 3 characters long");
                _username = value.Trim();
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Password cannot be empty");
                if (value.Length < 2)
                    throw new ArgumentException("Password must be at least 6 characters long");
                _password = value;
            }
        }

        public string FullName => $"{FirstName} {LastName}";
    }
}
