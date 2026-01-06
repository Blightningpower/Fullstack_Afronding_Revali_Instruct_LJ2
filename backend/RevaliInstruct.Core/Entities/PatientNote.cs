namespace RevaliInstruct.Core.Entities
{
    public class PatientNote
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public int AuthorUserId { get; set; }
        public User? Author { get; set; }
        public DateTime Timestamp { get; set; }
        public string Content { get; set; } = string.Empty;
    }
}