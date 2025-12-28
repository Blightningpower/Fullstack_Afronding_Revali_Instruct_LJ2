using RevaliInstruct.Core.Entities;

namespace RevaliInstruct.Api.Dtos
{
    public class PatientListItemDto
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public DateTime? StartDate { get; set; }
        public PatientStatus Status { get; set; }
        public string StatusLabel { get; set; } = string.Empty;
    }
}