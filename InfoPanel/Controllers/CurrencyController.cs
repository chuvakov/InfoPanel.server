using InfoPanel.Dto.Currency;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace InfoPanel.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CurrencyController : ControllerBase
{
    [HttpGet]
    public async Task<DailyCourseCurrenciesDto> GetDailyCourses()
    {
        using (var httpClient = new HttpClient())
        {
            httpClient.BaseAddress = new Uri("https://www.cbr-xml-daily.ru/");
            var response = await httpClient.GetAsync("daily_json.js");
            var responseContent = await response.Content.ReadAsStringAsync();

            var jObject = JObject.Parse(responseContent);
            return new DailyCourseCurrenciesDto
            {
                Dollar = GetDailyCurrency(jObject["Valute"]["USD"]),
                Euro = GetDailyCurrency(jObject["Valute"]["EUR"]),
                Yuan = GetDailyCurrency(jObject["Valute"]["CNY"])
            };
        }
    }

    private DailyCurrencyDto GetDailyCurrency(JToken currency)
    {
        var value = Math.Round(double.Parse(currency["Value"].ToString()), 2);
        var previousValue = Math.Round(double.Parse(currency["Previous"].ToString()), 2);
        var difference = Math.Round(value - previousValue, 2);
        
        return new DailyCurrencyDto
        {
            Value = value,
            Difference = difference
        };
    }
}