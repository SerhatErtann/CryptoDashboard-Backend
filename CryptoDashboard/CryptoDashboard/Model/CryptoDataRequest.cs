using CryptoDashboard.Services;

namespace CryptoDashboard.Model
{
    public class CryptoDataRequest
    {
     
        public string? CoinName { get; set; }
        public CryptoService.Period? Period { get; set; }
        public int? range { get; set; }
        public DateTime? startDate { get; set; }
        public DateTime? endDate { get; set; }
        public decimal? minPrice { get; set; }
        public decimal? maxPrice { get; set; }
        public string? sortColumn { get; set; }
        public string? sortOrder { get; set; }

    }
}
