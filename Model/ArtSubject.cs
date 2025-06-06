namespace oop_coursework.Models
{
    public class ArtSubject : Subject
    {
        public override required string Name { get; set; } = "Art";

        public ArtSubject()
        {
            Credits = 10;
        }
    }
}
