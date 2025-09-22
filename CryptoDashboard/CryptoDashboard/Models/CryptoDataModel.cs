namespace CryptoDashboard.Models
{
    public class CryptoDataModel
    {
        public DateTime Date { get; set; }
        public decimal Price { get; set; }
        public decimal Open { get; set; }
        public decimal High { get; set; }
        public decimal Low { get; set; }
        public decimal Vol { get; set; }
        public decimal Change_percent { get; set; }
        public string CoinName { get; set; } = string.Empty;
    }
}
