namespace Known.Sample.Models;

public class WeatherForecast
{
    [Column(Width = 120, IsQuery = true)]
    [Form(Type = nameof(FieldType.Date))]
    [DisplayName("日期")]
    public DateTime? Date { get; set; }

    [Column(Width = 150)]
    [Form]
    [DisplayName("温度C")]
    public int TemperatureC { get; set; }

    [Column(Width = 120)]
    [Form]
    [DisplayName("摘要")]
    public string Summary { get; set; }

    [Column(Width = 150)]
    [DisplayName("温度F")]
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

    [Column(Width = 250)]
    [DisplayName("信息")]
    public WeatherInfo Info { get; set; }
}

public class WeatherInfo
{
    public DateTime? Date1 { get; set; }
    public string Summary1 { get; set; }
}