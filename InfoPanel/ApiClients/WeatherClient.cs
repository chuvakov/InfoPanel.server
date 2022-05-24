using InfoPanel.ApiClients.Infrastructure;
using InfoPanel.Dto.Weather;
using Newtonsoft.Json.Linq;

namespace InfoPanel.ApiClients;

public class WeatherClient : IWeatherClient 
{
    private readonly HttpClient _httpClient;
    private readonly string _token;
    
    private readonly Dictionary<string, string> _wheatherTranslates = new Dictionary<string, string>()
    {
        {"Thunderstorm", "Гроза"},
        {"Drizzle", "Моросит"},
        {"Rain", "Дождь"},
        {"Snow", "Снег"},
        {"Mist", "Туман"},
        {"Smoke", "Дым"},
        {"Haze", "Туман"},
        {"Dust", "Пыль"},
        {"Fog", "Туманость"},
        {"Clear", "Ясно"},
        {"Clouds", "Облачно"}
    };
           
    public WeatherClient(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _token = configuration.GetSection("ApiClients:WeatherClient")["Token"];
    }
    
    public async Task<WeatherDto> GetWeather(double lat, double lon)
    {
        using(_httpClient)
        {
            var response = await _httpClient.GetAsync(@$"data/2.5/weather?appid={_token}&lat={lat}&lon={lon}&units=metric");

            var responseContent = await response.Content.ReadAsStringAsync();
            var jObject = JObject.Parse(responseContent);
            var unixTime = long.Parse(jObject["dt"].ToString());
            return new WeatherDto
            {
                Name = _wheatherTranslates[jObject["weather"][0]["main"].ToString()],
                Temp = double.Parse(jObject["main"]["temp"].ToString()),
                WindSpeed = double.Parse(jObject["wind"]["speed"].ToString()),
                Time = TimeZoneInfo.ConvertTime(DateTimeOffset.FromUnixTimeSeconds(unixTime),TimeZoneInfo.Local).ToString("HH:mm"),
                Humidity = int.Parse(jObject["main"]["humidity"].ToString()),
                Icon = jObject["weather"][0]["icon"].ToString()
            };
        }
    }
}