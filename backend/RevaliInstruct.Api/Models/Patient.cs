using System;

namespace RevaliInstruct.Core.Entities
{
    public class Patient
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public string? Diagnosis { get; set; }
        public PatientStatus Status { get; set; } = PatientStatus.IntakePlanned;
        public string? Notes { get; set; }

        public int AssignedDoctorUserId { get; set; }
        public User? AssignedDoctor { get; set; }

        public IntakeRecord? Intake { get; set; }
        public ICollection<ExerciseAssignment> ExerciseAssignments { get; set; } = new List<ExerciseAssignment>();
        public ICollection<PainEntry> PainEntries { get; set; } = new List<PainEntry>();
        public ICollection<ActivityLogEntry> ActivityLogs { get; set; } = new List<ActivityLogEntry>();
        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
        public ICollection<InvoiceItem> InvoiceItems { get; set; } = new List<InvoiceItem>();
        public ICollection<PatientNote> PatientNotes { get; set; } = new List<PatientNote>();
        public ICollection<AccessoryAdvice> AccessoryAdvices { get; set; } = new List<AccessoryAdvice>();

        public DateTime StartDate { get; set; } = DateTime.Now;
    }

    public class IntakeRecord
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public Patient? Patient { get; set; }
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public string Diagnosis { get; set; } = string.Empty;
        public string InjurySeverity { get; set; } = string.Empty;
        public string InitialGoals { get; set; } = string.Empty;
        public bool IsLocked { get; set; } = true;
    }

    public class Exercise
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
    }

    public class ExerciseAssignment
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public Patient? Patient { get; set; }
        public int ExerciseId { get; set; }
        public Exercise? Exercise { get; set; }
        public int? Repetitions { get; set; }
        public int? Sets { get; set; }
        public string? Frequency { get; set; }
        public TimeSpan? Duration { get; set; }
        public DateTime StartDateUtc { get; set; }
        public DateTime? EndDateUtc { get; set; }
        public bool ClientCheckedOff { get; set; } = false;
        public DateTime AssignedAtUtc { get; set; } = DateTime.UtcNow;
        public int AssignedByUserId { get; set; }
    }

    public class PainEntry
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public Patient? Patient { get; set; }
        public DateTime RecordedAtUtc { get; set; }
        public int Score { get; set; }
        public string? Location { get; set; }
        public string? Note { get; set; }
    }

    public class ActivityLogEntry
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public Patient? Patient { get; set; }
        public DateTime LoggedAtUtc { get; set; }
        public string Activity { get; set; } = string.Empty;
        public string? Details { get; set; }
    }

    public enum AppointmentStatus { Planned, Completed, Cancelled }
    public class Appointment
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public Patient? Patient { get; set; }
        public DateTime StartUtc { get; set; }
        public TimeSpan Duration { get; set; }
        public string Type { get; set; } = "Consult";
        public AppointmentStatus Status { get; set; } = AppointmentStatus.Planned;
        public int CreatedByUserId { get; set; }
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    }

    public enum ClaimStatus { Pending, Declared }
    public class InvoiceItem
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public Patient? Patient { get; set; }
        public DateTime DateUtc { get; set; }
        public string TreatmentType { get; set; } = "Consult";
        public decimal Amount { get; set; }
        public ClaimStatus Status { get; set; } = ClaimStatus.Pending;
        public int CreatedByUserId { get; set; }
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    }

    public class PatientNote
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public Patient? Patient { get; set; }
        public int AuthorUserId { get; set; }
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAtUtc { get; set; }
        public string Text { get; set; } = string.Empty;
    }

    public class AuditLog
    {
        public int Id { get; set; }
        public DateTime TimestampUtc { get; set; } = DateTime.UtcNow;
        public string Action { get; set; } = string.Empty;
        public string TableName { get; set; } = string.Empty;
        public string EntityId { get; set; } = string.Empty;
        public int? UserId { get; set; }
    }

    public class AccessoryAdvice
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public Patient? Patient { get; set; }

        // HuisartsID uit dataset (geen FK)
        public int GPUserId { get; set; }

        public string Name { get; set; } = string.Empty;
        public DateTime AdviceDateUtc { get; set; }
        public string ExpectedUsagePeriod { get; set; } = string.Empty; // bijv. '6 weken', 'Onbepaald'
        public string Status { get; set; } = "Actief"; // 'Actief' of 'Afgerond'
    }
}