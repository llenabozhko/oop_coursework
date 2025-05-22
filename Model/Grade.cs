namespace oop_coursework.Models
{
    public class Grade
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public int SubjectId { get; set; }
        public double Score { get; set; }
        public Subject Subject { get; set; }
    }
}
