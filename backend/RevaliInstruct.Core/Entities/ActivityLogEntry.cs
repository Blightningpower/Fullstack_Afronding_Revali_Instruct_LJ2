namespace RevaliInstruct.Core.Entities
{
    public class ActivityLogEntry
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public DateTime Timestamp { get; set; }
        public string Activity { get; set; } = string.Empty;
    }
}