namespace oop_coursework.Models
{
    public class EnglishSubject : Subject
    {
        public override required string Name { get; set; } = "English";

        public EnglishSubject()
        {
            IsExam = true;  // English is always an exam
            Credits = 4;    // Default credits for English
        }
    }
}
