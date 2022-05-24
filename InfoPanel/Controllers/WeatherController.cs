using System.Net.Http.Headers;
using System.Text;
using System.Text.Json.Serialization;
using InfoPanel.ApiClients;
using InfoPanel.ApiClients.Infrastructure;
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
    private readonly ILocationClient _locationClient;
    private readonly IWeatherClient _weatherClient;

    public WeatherController(ILocationClient locationClient, IWeatherClient weatherClient)
    {
        _locationClient = locationClient;
        _weatherClient = weatherClient;
    }

    /// <summary>
    /// Получение адреса
    /// </summary>
    /// <param name="input">Строка запроса адреса</param>
    /// <returns></returns>
    [HttpGet("[action]")]
    public async Task<IEnumerable<LocationDto>> GetLocations([FromQuery]GetLocationsInput input)
    {
        return await _locationClient.GetLocations(input);
    }
    
    /// <summary>
    /// Получение погоды по координатам
    /// </summary>
    /// <param name="lat">широта</param>
    /// <param name="lon">долгота</param>
    /// <returns></returns>
    [HttpGet]
    public async Task<WeatherDto> Get(double lat, double lon)
    {
        return await _weatherClient.GetWeather(lat, lon);
    }
}

