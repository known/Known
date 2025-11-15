namespace Known.Sample.Services;

[Import(typeof(Weather_Forecast))]
class WeatherImport(ImportContext context) : ImportBase<Weather_Forecast>(context)
{
    public override void InitColumns()
    {
        AddColumn(c => c.Date);
        AddColumn(c => c.TemperatureC);
        AddColumn(c => c.Summary);
    }
}