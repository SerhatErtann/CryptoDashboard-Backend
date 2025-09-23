using CryptoDashboard.Enums;
using CryptoDashboard.Services;

namespace CryptoDashboard.Models
{
    public class CryptoDataRequest
    {
        public string? CoinName { get; set; }
        public PeriodType? Period { get; set; }
        public int? range { get; set; }
        public DateTime? startDate { get; set; }
        public DateTime? endDate { get; set; }
        public decimal? minPrice { get; set; }
        public decimal? maxPrice { get; set; }
        public string? sortColumn { get; set; }
        public string? sortOrder { get; set; }

        public DateTime? StartDateUtc =>
            startDate.HasValue
                ? DateTime.SpecifyKind(startDate.Value, DateTimeKind.Utc)
                : null;

        public DateTime? EndDateUtc =>
            endDate.HasValue
                ? DateTime.SpecifyKind(endDate.Value, DateTimeKind.Utc)
                : null;
    }
}
