using CryptoDashboard.Models;
using CryptoDashboard.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using CryptoDashboard.Constant;


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

        [HttpGet(Endpoints.CryptoDashboard.Data)]
        public IActionResult GetData(string CoinName, DateTime startDate, DateTime endDate)
        {
            try
            {
                var result = _service.GetCryptoData(CoinName, startDate, endDate);
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


        [HttpGet(Endpoints.CryptoDashboard.DataFiltered)]
        public async Task<IActionResult> GetDataFiltered([FromQuery] CryptoDataRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.CoinName))
                    return BadRequest("Coin name is required.");

                List<CryptoDataModel> cryptoResult = await _service.GetCryptoDataFiltered(request);

                if (cryptoResult.Count == 0)
                {
                    return NotFound(new { message = "No data found in the selected range." });
                }

                return Ok(cryptoResult);
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




        [HttpGet(Endpoints.CryptoDashboard.Stats)]
        public IActionResult GetStats(DateTime startDate, DateTime endDate)
        {
            var result = _service.GetCryptoStats(startDate, endDate);

            _mongoService.LogDateRange(startDate, endDate);

            return Ok(result);
        }
    }
}
