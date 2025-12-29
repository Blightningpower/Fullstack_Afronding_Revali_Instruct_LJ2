using System;
using System.Collections.Generic;

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
        public string? Notes { get; set; }
        public DateTime? StartDate { get; set; }

        // De ingelogde Revalidatiearts
        public int AssignedDoctorUserId { get; set; }
        public User? AssignedDoctor { get; set; }

        // De Verwijzend Huisarts of Specialist
        public int? ReferringDoctorUserId { get; set; }
        public User? ReferringDoctor { get; set; }

        public ICollection<PainEntry> PainEntries { get; set; } = new List<PainEntry>();
        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
        public ICollection<ExerciseAssignment> Exercises { get; set; } = new List<ExerciseAssignment>();
        public ICollection<ActivityLogEntry> ActivityLogs { get; set; } = new List<ActivityLogEntry>();
        public ICollection<Medication> Medications { get; set; } = new List<Medication>();
        public ICollection<AccessoryAdvice> AccessoryAdvices { get; set; } = new List<AccessoryAdvice>();
    }

    public class PainEntry
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public DateTime Timestamp { get; set; } // Match met Seeding regel 113-115
        public int Score { get; set; }
        public string? Note { get; set; }
    }

    public class Exercise
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string? Description { get; set; }
        public string? VideoUrl { get; set; }
    }

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
        public Exercise? Exercise { get; set; }
        public int Repetitions { get; set; }
        public int Sets { get; set; }
        public string Frequency { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public bool ClientCheckedOff { get; set; }
        public DateTime? CheckedOffAt { get; set; }
    }

    public class ActivityLogEntry
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public DateTime Timestamp { get; set; }
        public string Activity { get; set; } = string.Empty;
    }

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

    public class AccessoryAdvice
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public int HuisartsId { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime AdviceDate { get; set; }
        public string ExpectedUsagePeriod { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }

    public class Appointment
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public int DoctorId { get; set; } 
        public DateTime AppointmentDateTime { get; set; }
        public int DurationMinutes { get; set; }
        public string Type { get; set; } = string.Empty; 
        public string Status { get; set; } = string.Empty;
    }

    public class AuditLog { public int Id { get; set; } }
}