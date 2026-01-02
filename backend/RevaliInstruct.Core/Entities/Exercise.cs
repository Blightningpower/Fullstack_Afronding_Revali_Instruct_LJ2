namespace RevaliInstruct.Core.Entities
{
    public class Exercise
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string? Description { get; set; }
        public string? VideoUrl { get; set; }
    }
}