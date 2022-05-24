using InfoPanel.Dto.Weather;

namespace InfoPanel.ApiClients.Infrastructure;

public interface IWeatherClient
{
    Task<WeatherDto> GetWeather(double lat, double lon);
}