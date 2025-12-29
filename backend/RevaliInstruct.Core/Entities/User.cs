namespace RevaliInstruct.Core.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Role { get; set; } = "User";
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;

        public int? OrganisatieId { get; set; }
        public ICollection<Patient> Patients { get; set; } = new List<Patient>();
    }
}