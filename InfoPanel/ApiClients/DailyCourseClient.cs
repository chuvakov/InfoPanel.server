using InfoPanel.ApiClients.Infrastructure;
using InfoPanel.Dto.Currency;
using Newtonsoft.Json.Linq;

namespace InfoPanel.ApiClients;

public class DailyCourseClient : IDailyCourseClient
{
    private readonly HttpClient _httpClient;

    public DailyCourseClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<DailyCourseCurrenciesDto> GetDailyCourses()
    {
        using (_httpClient)
        {
            var response = await _httpClient.GetAsync("daily_json.js");
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
}