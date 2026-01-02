namespace RevaliInstruct.Core.Entities
{
    public class PainEntry
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public DateTime Timestamp { get; set; }
        public int Score { get; set; }
        public string? Note { get; set; }
    }
}