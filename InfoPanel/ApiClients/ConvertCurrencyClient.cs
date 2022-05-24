using InfoPanel.ApiClients.Infrastructure;
using Newtonsoft.Json.Linq;

namespace InfoPanel.ApiClients;

public class ConvertCurrencyClient : IConvertCurrencyClient
{
    private readonly HttpClient _httpClient;

    public ConvertCurrencyClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<decimal> Convert(string from, string to, decimal sum)
    {
        using (_httpClient)
        {
            var response = await _httpClient.GetAsync($"/api/v7/convert?q={from}_{to}&compact=ultra&apiKey=ed8fce9240ebd9cad9db");
                    
            var responseContent = await response.Content.ReadAsStringAsync();
            var jObject = JObject.Parse(responseContent);
                    
            var value = jObject[$"{from}_{to}"];
            var result = decimal.Parse(value.ToString()) * sum;
                    
            return Math.Round(result, 2);
        }
    }
    
    public async Task<IEnumerable<string>> GetAll()
    {
        using (_httpClient)
        {
            var response = await _httpClient.GetAsync("/api/v7/currencies?apiKey=ed8fce9240ebd9cad9db");
            var responseContent = await response.Content.ReadAsStringAsync();

            var jObject = JObject.Parse(responseContent);
            var currencies = jObject["results"].ToObject<Dictionary<string, JToken>>().Keys;
            return currencies;
        }
    }
}