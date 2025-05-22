namespace oop_coursework.Models
{
    public class ArtSubject : Subject
    {
        public override string Name => "Art";

        public ArtSubject()
        {
            Credits = 10;
        }
    }
}
