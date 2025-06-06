namespace oop_coursework.Models
{
    public class EnglishSubject : Subject
    {
        public override required string Name { get; set; } = "English";

        public EnglishSubject()
        {
            Credits = 10;
        }
    }
}
