using InfoPanel.Dto.Weather;

namespace InfoPanel.ApiClients.Infrastructure;

public interface ILocationClient
{
    Task<IEnumerable<LocationDto>> GetLocations(GetLocationsInput input);
}