namespace Sample.Models;

public class WeatherForecast
{
    [Column(Width = 120, IsQuery = true)]
    [Form(Type = nameof(FieldType.Date))]
    [DisplayName("日期")]
    public DateTime? Date { get; set; }

    [Column(Width = 150)]
    [Form]
    public int TemperatureC { get; set; }

    [Column(Width = 120, Type = FieldType.File)]
    [Form]
    public string Summary { get; set; }

    [Column(Width = 150)]
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

    public WeatherInfo Info { get; set; }
}

public class WeatherInfo
{
    public DateTime? Date1 { get; set; }
    public string Summary1 { get; set; }
}