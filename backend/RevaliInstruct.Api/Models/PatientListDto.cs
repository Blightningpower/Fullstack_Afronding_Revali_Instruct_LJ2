namespace RevaliInstruct.Api.Models
{
    public class PatientListDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public DateTime DateOfBirth { get; set; }
        public DateTime StartDate { get; set; }
        public string Status { get; set; } = "";
    }
}