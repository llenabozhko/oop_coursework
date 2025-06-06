namespace oop_coursework.Models
{
    public class ArtSubject : Subject
    {
        public override required string Name { get; set; } = "Мистецтво";

        public ArtSubject()
        {
            Credits = 10;
        }
    }
}
