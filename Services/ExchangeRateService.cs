// Services/ExchangeRateService.cs
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

public class ExchangeRateService
{
    private static readonly HttpClient client = new HttpClient();
    private const string BaseUrl = "https://api.frankfurter.app/";

    public async Task<JObject> GetLatestRates(string baseCurrency)
    {
        var response = await client.GetStringAsync($"{BaseUrl}latest?from={baseCurrency}");
        return JObject.Parse(response);
    }

    public async Task<JObject> ConvertCurrency(string from, string to, decimal amount)
    {
        var response = await client.GetStringAsync($"{BaseUrl}latest?from={from}&to={to}&amount={amount}");
        return JObject.Parse(response);
    }

    public async Task<JObject> GetHistoricalRates(string baseCurrency, DateTime startDate, DateTime endDate)
    {
        var response = await client.GetStringAsync($"{BaseUrl}{startDate:yyyy-MM-dd}..{endDate:yyyy-MM-dd}?from={baseCurrency}");
        return JObject.Parse(response);
    }
}
