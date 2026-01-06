using System.ComponentModel.DataAnnotations;

namespace RevaliInstruct.Api.Dtos
{
    public class IntakeDto
    {
        [Required(ErrorMessage = "Diagnose is verplicht")]
        public string Diagnosis { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ernst is verplicht")]
        public string Severity { get; set; } = string.Empty;

        [Required(ErrorMessage = "Behandeldoelen zijn verplicht")]
        public string InitialGoals { get; set; } = string.Empty;
    }
}