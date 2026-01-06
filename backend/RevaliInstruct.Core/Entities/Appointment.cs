namespace RevaliInstruct.Core.Entities
{
    public class Appointment
    {
        public int Id { get; set; }
        public int? PatientId { get; set; }
        public int? DoctorId { get; set; }
        public DateTime DateTime { get; set; }
        public int Duration { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Status { get; set; } = "Gepland";
    }
}