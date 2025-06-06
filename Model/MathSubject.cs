namespace oop_coursework.Models
{
    public class MathSubject : Subject
    {
        public override required string Name { get; set; } = "Математика";

        public MathSubject()
        {
            Credits = 10;
            IsExam = true;
        }
    }
}
