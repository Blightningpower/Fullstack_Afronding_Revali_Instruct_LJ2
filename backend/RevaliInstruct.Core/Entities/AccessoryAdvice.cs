namespace RevaliInstruct.Core.Entities
{
    public class AccessoryAdvice
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public int HuisartsId { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime AdviceDate { get; set; }
        public string ExpectedUsagePeriod { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }
}