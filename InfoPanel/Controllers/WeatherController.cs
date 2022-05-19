using System.Net.Http.Headers;
using System.Text;
using System.Text.Json.Serialization;
using InfoPanel.Dto;
using InfoPanel.Dto.Weather;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace InfoPanel.Controllers;

[ApiController] 
[Route("api/[controller]")] 
public class WeatherController : ControllerBase
{
    //private readonly HttpClient _httpClient;
    
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
    
    [HttpGet("GetLocations")]
    public async Task<IEnumerable<LocationDto>> GetLocations([FromQuery]GetLocationsInput input)
    {
        var result = new List<LocationDto>();
        using (var client = new HttpClient())
        {
            client.BaseAddress = new Uri("https://suggestions.dadata.ru/");

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Token", "722f97d320f6b6271757ee22f84394b046f739b5");
            
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            
            var requestContent = new StringContent(
                JsonConvert.SerializeObject(input,
                    new JsonSerializerSettings() {ContractResolver = new CamelCasePropertyNamesContractResolver()}), 
                Encoding.UTF8, 
                "application/json");

            var response = await client.PostAsync(@"suggestions/api/4_1/rs/suggest/address", requestContent);

            var responseContent = await response.Content.ReadAsStringAsync();
            var jObject = JObject.Parse(responseContent);

            foreach (var location in jObject["suggestions"])
            {
                var lat = location["data"]["geo_lat"].ToString();
                var lon = location["data"]["geo_lon"].ToString();

                if (string.IsNullOrEmpty(lat) && string.IsNullOrEmpty(lon))
                {
                    continue;
                }
                    
                result.Add(new LocationDto()
                {
                    Name = location["value"].ToString(),
                    Lat = double.Parse(lat),
                    Lon = double.Parse(lon),
                });
            }

            return result;
        }
    }
    
    [HttpGet]
    public async Task<WeatherDto> Get(double lat, double lon)
    {
        using (var client = new HttpClient())
        {
            client.BaseAddress = new Uri("https://api.openweathermap.org");
            string appid = "2f314e0a2465820a76202a9ed015f4de";

            var response = await client.GetAsync(@$"data/2.5/weather?appid={appid}&lat={lat}&lon={lon}&units=metric");

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

