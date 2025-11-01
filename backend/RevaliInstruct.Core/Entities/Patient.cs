namespace RevaliInstruct.Core.Entities
{

    public class Patient
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public DateTime StartDate { get; set; }
        public PatientStatus Status { get; set; }
        public string? Notes { get; set; }

        // Auditvelden
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; } = "seed";
        public DateTime? ModifiedAt { get; set; }
        public string? ModifiedBy { get; set; }
    }
}