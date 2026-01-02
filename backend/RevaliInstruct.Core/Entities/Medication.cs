namespace RevaliInstruct.Core.Entities
{
    public class Medication
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public int HuisartsId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Dosage { get; set; } = string.Empty;
        public string Frequency { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}