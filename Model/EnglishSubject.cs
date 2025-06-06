namespace oop_coursework.Models
{
    public class EnglishSubject : Subject
    {
        public override required string Name { get; set; } = "Англійська мова";

        public EnglishSubject()
        {
            Credits = 10;
        }
    }
}
