using RevaliInstruct.Core.Data.Enums;

namespace RevaliInstruct.Core.Entities
{
    public class Patient
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public DateTime BirthDate { get; set; }
        public string? Address { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public PatientStatus Status { get; set; } = PatientStatus.IntakePlanned;
        public DateTime? StartDate { get; set; }

        // De Revalidatiearts
        public int AssignedDoctorUserId { get; set; }
        public User? AssignedDoctor { get; set; }

        // De Verwijzend Huisarts of Specialist
        public int? ReferringDoctorUserId { get; set; }
        public User? ReferringDoctor { get; set; }

        // Navigatie-property voor ActiviteitlogEntries
        public List<ActivityLogEntry> ActivityLogEntries { get; set; } = new();

        public ICollection<PainEntry> PainEntries { get; set; } = new List<PainEntry>();
        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
        public ICollection<ExerciseAssignment> Exercises { get; set; } = new List<ExerciseAssignment>();
        public ICollection<PatientNote> PatientNotes { get; set; } = new List<PatientNote>();
        public ICollection<Medication> Medications { get; set; } = new List<Medication>();
        public ICollection<AccessoryAdvice> AccessoryAdvices { get; set; } = new List<AccessoryAdvice>();
        public ICollection<IntakeRecord> IntakeRecords { get; set; } = new List<IntakeRecord>();
        public ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();
        public ICollection<Declaration> Declarations { get; set; } = new List<Declaration>();
    }
}