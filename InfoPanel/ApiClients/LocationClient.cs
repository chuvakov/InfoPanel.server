using System.Net.Http.Headers;
using System.Text;
using InfoPanel.ApiClients.Infrastructure;
using InfoPanel.Dto.Weather;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace InfoPanel.ApiClients;

public class LocationClient : ILocationClient
{
    private readonly HttpClient _httpClient;
    private readonly string _token;

    public LocationClient(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _token = configuration.GetSection("ApiClients:LocationClient")["Token"];
    }
    
    public async Task<IEnumerable<LocationDto>> GetLocations(GetLocationsInput input)
    {
        var result = new List<LocationDto>();
        using (_httpClient)
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Token", _token);

            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var requestContent = new StringContent(
                JsonConvert.SerializeObject(input,
                    new JsonSerializerSettings() {ContractResolver = new CamelCasePropertyNamesContractResolver()}),
                Encoding.UTF8,
                "application/json");

            var response = await _httpClient.PostAsync(@"suggestions/api/4_1/rs/suggest/address", requestContent);

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