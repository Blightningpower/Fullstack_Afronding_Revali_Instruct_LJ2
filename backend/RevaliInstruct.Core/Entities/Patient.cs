using System;
using System.Collections.Generic;

namespace RevaliInstruct.Core.Entities
{
    public class Patient
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public DateTime BirthDate { get; set; } // Match met Seeding regel 88-92
        public string? Address { get; set; } 
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public PatientStatus Status { get; set; } = PatientStatus.IntakePlanned;
        public string? Notes { get; set; }
        public DateTime? StartDate { get; set; }
        public int AssignedDoctorUserId { get; set; }
        public User? AssignedDoctor { get; set; }

        public IntakeRecord? Intake { get; set; }
        public ICollection<PainEntry> PainEntries { get; set; } = new List<PainEntry>();
        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
        public ICollection<InvoiceItem> InvoiceItems { get; set; } = new List<InvoiceItem>();
        public ICollection<ExerciseAssignment> Exercises { get; set; } = new List<ExerciseAssignment>();
        public ICollection<ActivityLogEntry> ActivityLogs { get; set; } = new List<ActivityLogEntry>();
        public ICollection<AccessoryAdvice> AccessoryAdvices { get; set; } = new List<AccessoryAdvice>();
    }

    public class Appointment
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public int DoctorId { get; set; }
        public DateTime AppointmentDateTime { get; set; } 
        public int DurationMinutes { get; set; }
        public string Type { get; set; } = string.Empty;
        // Veranderd naar string om "Afgerond" uit de Seeding te accepteren (Error CS0029)
        public string Status { get; set; } = string.Empty; 
    }

    public class InvoiceItem
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public string Type { get; set; } = string.Empty;
        public int AuthorId { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        // Veranderd naar string om "Nieuw" uit de Seeding te accepteren (Error CS0029)
        public string Status { get; set; } = string.Empty; 
    }

    public class PainEntry
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public DateTime Timestamp { get; set; } // Match met Seeding regel 113-115
        public int Score { get; set; }
        public string? Note { get; set; }
    }

    // Overige placeholders voor build-stabiliteit
    public class IntakeRecord { public int Id { get; set; } public int PatientId { get; set; } public Patient? Patient { get; set; } public int DoctorId { get; set; } public string Diagnosis { get; set; } = ""; public string Severity { get; set; } = ""; public string Goals { get; set; } = ""; public DateTime Date { get; set; } }
    public class Exercise { public int Id { get; set; } public string Name { get; set; } = ""; public string? Description { get; set; } public string? VideoUrl { get; set; } }
public class PatientNote
    {
        public int Id { get; set; }
        public int PatientId { get; set; } // Oplossing voor error CS0117 (regel 120, 121)
        public int AuthorId { get; set; }
        public DateTime Timestamp { get; set; }
        public string Content { get; set; } = string.Empty;
    }

    public class ExerciseAssignment
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public int ExerciseId { get; set; }
        public Exercise? Exercise { get; set; } // Oplossing voor error CS1061 (regel 58)
    }

    public class ActivityLogEntry 
    { 
        public int Id { get; set; } 
        public int PatientId { get; set; } 
    }

    public class AccessoryAdvice
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public int HuisartsId { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime AdviceDate { get; set; }
        public string Duration { get; set; } = string.Empty;
        public string Status { get; set; } = "Actief";
    }

    public class AuditLog { public int Id { get; set; } }
}