namespace RevaliInstruct.Api.Dtos
{
    public class LoginRequestDto
    {
        [System.ComponentModel.DataAnnotations.Required]
        public string? Username { get; set; }

        [System.ComponentModel.DataAnnotations.Required]
        public string? Password { get; set; }
    }
}