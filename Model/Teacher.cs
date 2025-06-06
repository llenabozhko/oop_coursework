using System.Collections.Generic;

namespace oop_coursework.Models
{
    public class Teacher : User
    {
        public required Subject Subject { get; set; }
    }
}
