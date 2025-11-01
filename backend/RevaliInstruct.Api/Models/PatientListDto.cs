namespace RevaliInstruct.Api.Models
{
    public class PatientListDto
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}