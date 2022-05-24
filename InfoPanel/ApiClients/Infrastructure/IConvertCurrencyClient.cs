namespace InfoPanel.ApiClients.Infrastructure;

public interface IConvertCurrencyClient
{
    Task<decimal> Convert(string from, string to, decimal sum);
    Task<IEnumerable<string>> GetAll();
}