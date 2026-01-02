namespace RevaliInstruct.Api.Dtos
{
    public class ExerciseAssignmentDto
    {
        public int ExerciseId { get; set; }
        public int Repetitions { get; set; }
        public int Sets { get; set; }
        public int Frequency { get; set; }
        public string? Notes { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}