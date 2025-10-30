namespace RevaliInstruct.Api.Models
{
    public class AuthResponse
    {
        public string Token { get; set; } = null!;
        public string Username { get; set; } = null!;
        public string Role { get; set; } = null!;
        public string? DisplayName { get; set; }
    }
}