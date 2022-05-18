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
public class WheatherController : ControllerBase
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
    
    [HttpPost("GetLocationsInput")]
    public async Task<IEnumerable<LocationDto>> GetLocations(GetLocationsInput input)
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
}

