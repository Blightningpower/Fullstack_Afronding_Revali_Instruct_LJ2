using System;

namespace RevaliInstruct.Core.Entities
{
    public class Patient
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public DateTime? DateOfBirth { get; set; }

        // startdatum van het traject
        public DateTime StartDate { get; set; }

        // status (enum PatientStatus)
        public PatientStatus Status { get; set; }

        public string? Notes { get; set; }

        // ---- Audit velden (toegevoegd om de context te laten compile- en werken) ----

        // wie heeft het record aangemaakt (username/id).
        public string? CreatedBy { get; set; }

        // wanneer is het aangemaakt
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // wie/wanneer is laatst aangepast — nullable omdat niet altijd aangepast
        public DateTime? ModifiedAt { get; set; }
        public string? ModifiedBy { get; set; }
    }
}