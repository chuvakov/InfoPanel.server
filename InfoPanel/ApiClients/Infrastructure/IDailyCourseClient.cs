using InfoPanel.Dto.Currency;

namespace InfoPanel.ApiClients.Infrastructure;

public interface IDailyCourseClient
{
    Task<DailyCourseCurrenciesDto> GetDailyCourses();
}