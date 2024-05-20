// Controllers/CurrencyConverterController.cs
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json.Linq;

namespace CurrencyConverterAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CurrencyConverterController : ControllerBase
    {
        private readonly ExchangeRateService _exchangeRateService;

        public CurrencyConverterController()
        {
            _exchangeRateService = new ExchangeRateService();
        }

        [HttpGet("latest")]
        public async Task<IActionResult> GetLatestRates([FromQuery] string baseCurrency)
        {
            try {
                var result = await _exchangeRateService.GetLatestRates(baseCurrency);
                var rates = result["rates"].ToObject<Dictionary<string, decimal>>();
                return Ok(rates);
            }
            catch (Exception ex)
            {
                return BadRequest("Something went wrong, please try again later.");
            }
        
        }

        [HttpGet("convert")]
        public async Task<IActionResult> ConvertCurrency([FromQuery] string from, [FromQuery] string to, [FromQuery] decimal amount)
        {
            try {
                var excludedCurrencies = new[] { "TRY", "PLN", "THB", "MXN" };

                if (excludedCurrencies.Contains(from.ToUpper()) || excludedCurrencies.Contains(to.ToUpper()))
                {
                    return BadRequest("Currency conversion for TRY, PLN, THB, and MXN is not supported.");
                }

                var result = await _exchangeRateService.ConvertCurrency(from, to, amount);

                var conversionResult = new
                {
                    Amount = result["amount"].ToObject<decimal>(),
                    Base = result["amount"].ToObject<string>(),
                    Date = result["date"].ToObject<string>(),
                    Rates = result["rates"].ToObject<Dictionary<string, decimal>>()
                };

                return Ok(conversionResult);

            }
            catch(Exception ex) {
                return BadRequest("Something went wrong, please try again later.");
            }
           
        }

        [HttpGet("historical")]
        public async Task<IActionResult> GetHistoricalRates([FromQuery] string baseCurrency, [FromQuery] DateTime startDate, [FromQuery] DateTime endDate, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try {

                var result = await _exchangeRateService.GetHistoricalRates(baseCurrency, startDate, endDate);

                var rates = result["rates"].ToObject<Dictionary<string, Dictionary<string, decimal>>>();
                var totalCount = rates.Count;
                var paginatedRates = rates.Skip((page - 1) * pageSize).Take(pageSize).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

                var paginatedResult = new
                {
                    TotalCount = totalCount,
                    Page = page,
                    PageSize = pageSize,
                    Rates = paginatedRates
                };

                return Ok(paginatedResult);
            }
            catch (Exception ex)
            {
                return BadRequest("Something went wrong, please try again later.");
            }
           
        }
    }
}
