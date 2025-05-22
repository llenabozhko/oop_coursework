namespace oop_coursework.Models
{
    public class MathSubject : Subject
    {
        public override string Name => "Mathematics";

        public MathSubject()
        {
            Credits = 10;
        }
    }
}
