using Microsoft.AspNetCore.Mvc;
using CryptoDashboard.Services;
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CryptoDashboard.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CryptoDashboardController : ControllerBase
    {
        private readonly CryptoService _service;
        private readonly MongoDbService _mongoService;

        
        public CryptoDashboardController(CryptoService service, MongoDbService mongoService)
        {
            _service = service;
            _mongoService = mongoService;
        }

        [HttpGet("Data")]
        public IActionResult GetData(string coinName, DateTime startDate, DateTime endDate)
        {
            try
            {
                var result = _service.GetCryptoData(coinName, startDate, endDate);
                _mongoService.LogDateRange(startDate, endDate);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = ex.Message,
                    stack = ex.StackTrace
                });
            }
        }
       [HttpGet("DataFiltered")]
public IActionResult GetDataFiltered(
    [FromQuery] string coinName,
    [FromQuery] string? period = null,
    [FromQuery] DateTime? startDate = null,
    [FromQuery] DateTime? endDate = null,
    [FromQuery] decimal? minPrice = null,
    [FromQuery] decimal? maxPrice = null,
    [FromQuery] string? sortColumn = null,
    [FromQuery] string? sortOrder = null
)
{
    try
    {
        if (string.IsNullOrWhiteSpace(coinName))
            return BadRequest("Coin name is required.");

        var result = _service.GetCryptoDataFiltered(
            coinName,
            period,
            startDate,
            endDate,
            minPrice,
            maxPrice,
            sortColumn,
            sortOrder
        );

        if (result.Count == 0)
        {
            return NotFound(new { message = "Seçilen aralıkta veri bulunamadı." });
        }

        return Ok(result);
    }
    catch (Exception ex)
    {
        return StatusCode(500, new
        {
            message = ex.Message,
            stack = ex.StackTrace
        });
    }
            //if (period == "weekly")
            //{
                
            //}




}


        [HttpGet("Stats")]
        public IActionResult GetStats(DateTime startDate, DateTime endDate)
        {
            var result = _service.GetCryptoStats(startDate, endDate);

            _mongoService.LogDateRange(startDate, endDate);

            return Ok(result);
        }
    }
}
