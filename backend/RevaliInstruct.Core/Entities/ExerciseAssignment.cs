namespace RevaliInstruct.Core.Entities
{
    public class ExerciseAssignment
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public int ExerciseId { get; set; }
        public Exercise? Exercise { get; set; }
        
        public int Repetitions { get; set; }
        public int Sets { get; set; }
        public int Frequency { get; set; }
        public string? Notes { get; set; }
        
        public bool ClientCheckedOff { get; set; }
        public DateTime? CheckedOffAt { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}