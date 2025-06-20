using System.Collections.Generic;

namespace oop_coursework.Models
{
    public class Student : User
    {
        public required string Specialty { get; set; }
        public List<Grade> Grades { get; set; } = new List<Grade>();
    }
}
