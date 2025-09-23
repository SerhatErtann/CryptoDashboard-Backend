using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace CryptoDashboard.Models
{
    [Table("CryptoPrice")] 
    public class CryptoPrice
    {
        [Column("SNo")]
        public int SNo { get; set; } // prımary key

        [Column("CoinName")]
        public string CoinName { get; set; } = string.Empty;


        [Column(TypeName = "timestamp without time zone")]
       
        public DateTime Date { get; set; }

        [Column("Price")]
        public decimal Price { get; set; }

        [Column("Open")]
        public decimal Open { get; set; }

        [Column("High")]
        public decimal High { get; set; }

        [Column("Low")]
        public decimal Low { get; set; }

        [Column("Vol")]
        public decimal Vol { get; set; }

        [Column("Change_percent")]
        public decimal ChangePercent { get; set; }
    }
}