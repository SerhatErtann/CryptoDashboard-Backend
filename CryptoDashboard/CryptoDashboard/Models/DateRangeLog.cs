namespace CryptoDashboard.Models
{
    public class DateRangeLog
    {
        public Guid Id { get; set; }     
        public DateTime BeginDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime RequestDate { get; set; }
    }
}
