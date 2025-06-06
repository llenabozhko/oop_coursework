namespace oop_coursework.Models
{
    public class MathSubject : Subject
    {
        public override required string Name { get; set; } = "Mathematics";

        public MathSubject()
        {
            Credits = 10;
        }
    }
}
