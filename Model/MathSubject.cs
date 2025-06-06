namespace oop_coursework.Models
{
    public class MathSubject : Subject
    {
        public override required string Name { get; set; } = "Mathematics";

        public MathSubject()
        {
            IsExam = true;  // Mathematics is always an exam
            Credits = 5;    // Default credits for Math
        }
    }
}
