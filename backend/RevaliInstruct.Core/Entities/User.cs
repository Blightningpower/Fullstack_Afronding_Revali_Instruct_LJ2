namespace RevaliInstruct.Core.Entities
{
    public class User
    {
        public int UserId { get; set; }   
        public string Username { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public string? Role { get; set; }
        public string? DisplayName { get; set; }

        // audit velden
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
    }
}