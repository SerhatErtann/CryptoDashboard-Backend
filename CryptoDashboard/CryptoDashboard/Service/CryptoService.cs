using CryptoDashboard.Constant;
using CryptoDashboard.DbContexts;
using CryptoDashboard.Enums;
using CryptoDashboard.Models;
using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL;
using static CryptoDashboard.Models.CryptoPrice;

namespace CryptoDashboard.Services
{
    public class CryptoService
    {
        private readonly CryptoDbContext _context;
        public CryptoService(CryptoDbContext context)
        { _context = context; }  
        public static DateTime FirstDateOfWeek(DateTime date)
        {
            int diff = (7 + (date.DayOfWeek - DayOfWeek.Monday)) % 7;
            return date.AddDays(-diff).Date;
        }

        public async Task<List<CryptoDataModel>> GetCryptoDataFiltered(CryptoDataRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.CoinName))
                    throw new ArgumentException("CoinName boş olamaz.");

                DateTime endDate = (request.endDate ?? DateTime.UtcNow).Date;
                int range = request.range ?? 30;
                DateTime startDate = (request.startDate ?? endDate.AddDays(-range)).Date;

                var query = _context.CryptoPrice
                    .Where(cp => cp.CoinName.ToLower().StartsWith(request.CoinName.ToLower()) &&
                                 cp.Date >= startDate && cp.Date <= endDate);

                if (request.minPrice.HasValue)
                    query = query.Where(cp => cp.Price >= request.minPrice.Value);
                if (request.maxPrice.HasValue)
                    query = query.Where(cp => cp.Price <= request.maxPrice.Value);

                var list = await query.ToListAsync();

                var grouped = list
                    .GroupBy(cp => request.Period switch
                    {
                        PeriodType.weekly => FirstDateOfWeek(cp.Date),
                        PeriodType.monthly => new DateTime(cp.Date.Year, cp.Date.Month, 1),
                        _ => cp.Date.Date
                    })
                    .Select(g => new CryptoDataModel
                    {
                        Date = g.Key,
                        Price = g.Average(x => x.Price),
                        Open = g.Average(x => x.Open),
                        High = g.Average(x => x.High),
                        Low = g.Average(x => x.Low),
                        Vol = g.Sum(x => x.Vol),
                        Change_percent = g.Average(x => x.Change_percent),
                        CoinName = g.First().CoinName
                    })
                    .OrderBy(x => x.Date)
                    .ToList();

                return grouped;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GetCryptoDataFiltered Hata: {ex.Message}");
                return new List<CryptoDataModel>();
            }
        }

       

        public async Task<List<CryptoDataModel>> GetCryptoData(string coinName, DateTime startDate, DateTime endDate)
        {
            try
            {
                return await _context.CryptoPrice
                    .Where(cp =>
                        cp.CoinName.ToLower().StartsWith(coinName.ToLower()) &&
                        cp.Date >= startDate &&
                        cp.Date <= endDate
                    )
                    .OrderBy(cp => cp.Date)
                    .Select(cp => new CryptoDataModel
                    {
                        Date = cp.Date,
                        Price = cp.Price,
                        Open = cp.Open,
                        High = cp.High,
                        Low = cp.Low,
                        Vol = cp.Vol,
                        Change_percent = cp.Change_percent,
                        CoinName = cp.CoinName
                    })
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GetCryptoData Hata: {ex.Message}");
                return new List<CryptoDataModel>();
            }
        }

        public async Task<object?> GetCryptoStats(DateTime startDate, DateTime endDate)
        {
            try
            {
                var stats = await _context.CryptoPrice
                    .Where(cp => cp.Date >= startDate && cp.Date <= endDate)
                    .GroupBy(cp => cp.CoinName)
                    .Select(g => new
                    {
                        CoinName = g.Key,
                        MinPrice = g.Min(x => x.Price),
                        MaxPrice = g.Max(x => x.Price),
                        AvgDiffPercent = g.Average(x =>
                            (x.Open != 0 ? ((x.Price - x.Open) / x.Open) * 100 : 0))
                    })
                    .OrderByDescending(x => x.AvgDiffPercent)
                    .FirstOrDefaultAsync();

                return stats;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GetCryptoStats Hata: {ex.Message}");
                return null;
            }
        }
    }
}