using InfoPanel.Dto.Currency;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace InfoPanel.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CurrencyController : ControllerBase
{
    /// <summary>
    /// Получение курсов валют за текущий день
    /// </summary>
    /// <returns></returns>
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

    /// <summary>
    /// Парс курса валют для ответа
    /// </summary>
    /// <param name="currency"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Конвертор валют
    /// </summary>
    /// <param name="from">из</param>
    /// <param name="to">во что</param>
    /// <param name="sum">сколько</param>
    /// <returns></returns>
    [HttpGet("[action]")]
    public async Task<decimal> Convert(string from, string to, decimal sum)
    {
        using (var httpClient = new HttpClient())
        {
            httpClient.BaseAddress = new Uri("https://free.currconv.com/");
            var response = await httpClient.GetAsync($"/api/v7/convert?q={from}_{to}&compact=ultra&apiKey=ed8fce9240ebd9cad9db");
            
            var responseContent = await response.Content.ReadAsStringAsync();
            var jObject = JObject.Parse(responseContent);
            
            var value = jObject[$"{from}_{to}"];
            var result = decimal.Parse(value.ToString()) * sum;
            
            return Math.Round(result, 2);
        }
    }
    
    /// <summary>
    /// Получение списка валют 
    /// </summary>
    /// <returns></returns>
    [HttpGet("[action]")]
    public async Task<IEnumerable<string>> GetAll()
    {
        using (var httpClient = new HttpClient())
        {
            httpClient.BaseAddress = new Uri("https://free.currconv.com/");
            var response = await httpClient.GetAsync("/api/v7/currencies?apiKey=ed8fce9240ebd9cad9db");
            var responseContent = await response.Content.ReadAsStringAsync();

            var jObject = JObject.Parse(responseContent);
            var currencies = jObject["results"].ToObject<Dictionary<string, JToken>>().Keys;
            return currencies;
        }
    }
}