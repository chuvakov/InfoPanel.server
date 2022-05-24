using InfoPanel.ApiClients;
using InfoPanel.ApiClients.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors();

//ApiClients - Список локаций
var locationClientOptions = new HttpClientOptions();
builder.Configuration.GetSection("ApiClients:LocationClient").Bind(locationClientOptions); //маппинг

builder.Services.AddHttpClient<ILocationClient, LocationClient>(opt =>
{
    opt.BaseAddress = locationClientOptions.BaseAddress; //внедрение зависимостей
});

//ApiClients - Погода
var weatherClientOptions = new HttpClientOptions();
builder.Configuration.GetSection("ApiClients:WeatherClient").Bind(weatherClientOptions);

builder.Services.AddHttpClient<IWeatherClient, WeatherClient>(opt =>
{
    opt.BaseAddress = weatherClientOptions.BaseAddress;
});

//ApiClients - Текущий курс валют
var dailyCourseClientOptions = new HttpClientOptions();
builder.Configuration.GetSection("ApiClients:DailyCourseClient").Bind(dailyCourseClientOptions);

builder.Services.AddHttpClient<IDailyCourseClient, DailyCourseClient>(opt =>
{
    opt.BaseAddress = dailyCourseClientOptions.BaseAddress;
});

//ApiClients - ConvertCurrency
var convertCurrencyOptions = new HttpClientOptions();
builder.Configuration.GetSection("ApiClients:ConvertCurrencyClient").Bind(convertCurrencyOptions);

builder.Services.AddHttpClient<IConvertCurrencyClient, ConvertCurrencyClient>(opt =>
{
    opt.BaseAddress = convertCurrencyOptions.BaseAddress;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors(builder => builder.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
app.UseAuthorization();
app.MapControllers();

app.Run();