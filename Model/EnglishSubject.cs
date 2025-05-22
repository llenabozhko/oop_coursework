namespace oop_coursework.Models
{
    public class EnglishSubject : Subject
    {
        public override string Name => "English";

        public EnglishSubject()
        {
            Credits = 10;
        }
    }
}
