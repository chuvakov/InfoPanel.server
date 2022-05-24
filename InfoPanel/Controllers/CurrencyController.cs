using InfoPanel.ApiClients.Infrastructure;
using InfoPanel.Dto.Currency;
using Microsoft.AspNetCore.Mvc;

namespace InfoPanel.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CurrencyController : ControllerBase
{
    private readonly IDailyCourseClient _dailyCourseClient;
    private readonly IConvertCurrencyClient _convertCurrencyClient;
    
    public CurrencyController(IDailyCourseClient dailyCourseClient, IConvertCurrencyClient convertCurrencyClient)
    {
        _dailyCourseClient = dailyCourseClient;
        _convertCurrencyClient = convertCurrencyClient;
    }

    /// <summary>
    /// Получение курсов валют за текущий день
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<DailyCourseCurrenciesDto> GetDailyCourses()
    {
        return await _dailyCourseClient.GetDailyCourses();
    }

    /// <summary>
    /// Конвертор валют
    /// </summary>
    /// <param name="from">из</param>
    /// <param name="to">во что</param>
    /// <param name="sum">сколько</param>
    /// <returns></returns>
    [HttpGet("[action]")]
    public async Task<decimal> Convert(string from, string to, decimal sum)
    {
        return await _convertCurrencyClient.Convert(from, to, sum);
    }
    
    /// <summary>
    /// Получение списка валют 
    /// </summary>
    /// <returns></returns>
    [HttpGet("[action]")]
    public async Task<IEnumerable<string>> GetAll()
    {
        return await _convertCurrencyClient.GetAll();
    }
}