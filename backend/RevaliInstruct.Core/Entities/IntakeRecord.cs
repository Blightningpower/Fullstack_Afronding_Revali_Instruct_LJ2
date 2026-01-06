using System.Text.Json.Serialization;

namespace RevaliInstruct.Core.Entities
{
    public class IntakeRecord
    {
        public int Id { get; set; }
        public int PatientId { get; set; }

        [JsonIgnore]
        public Patient? Patient { get; set; }
        public int DoctorId { get; set; }
        public string Diagnosis { get; set; } = string.Empty;
        public string Severity { get; set; } = string.Empty;
        public string InitialGoals { get; set; } = string.Empty;
        public DateTime Date { get; set; }
    }
}