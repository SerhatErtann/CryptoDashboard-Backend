using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace CryptoDashboard.Models
{
    [Table("CryptoPrice")] 
    public class CryptoPrice
    {
        //public int Id { get; set; }

        [Column("coinname")]
        public string CoinName { get; set; } = string.Empty;

        [Column("date")]
        public DateTime Date { get; set; }

        [Column("price")]
        public decimal Price { get; set; }

        [Column("open")]
        public decimal Open { get; set; }

        [Column("high")]
        public decimal High { get; set; }

        [Column("low")]
        public decimal Low { get; set; }

        [Column("vol")]
        public decimal Vol { get; set; }

        [Column("change_percent")]
        public decimal Change_percent { get; set; }
    }
}