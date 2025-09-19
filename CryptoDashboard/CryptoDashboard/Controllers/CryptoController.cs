using CryptoDashboard.Model;
using CryptoDashboard.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;

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
        public IActionResult GetDataFiltered([FromQuery] CryptoDataRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.CoinName))
                    return BadRequest("Coin name is required.");

                var result = _service.GetCryptoDataFiltered(request);

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
