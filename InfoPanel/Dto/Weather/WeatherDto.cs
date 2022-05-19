namespace InfoPanel.Dto.Weather;

public class WeatherDto
{
    public string Name { get; set; }
    public double Temp { get; set; }
    public double WindSpeed { get; set; }
    public string Time { get; set; }
    public int Humidity { get; set; }
    public string Icon { get; set; }
}