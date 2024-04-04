namespace Known.BMap;

public class Location
{
    public int Accuracy { get; set; }
    public object Altitude { get; set; }
    public object AltitudeAccuracy { get; set; }
    public object Heading { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public object Speed { get; set; }
    public object Timestamp { get; set; }
    public Point Point { get; set; }
    public Address Address { get; set; }
    public int Status { get; set; } = 0;
    public string Error { get; set; } = string.Empty;

    public override string ToString()
    {
        string msg = Status != 0 ? $"Error{Status}" : $"{Address}";

        return msg;
    }
}

public class Address
{
    public string Country { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public int City_code { get; set; }
    public string District { get; set; } = string.Empty;
    public string Province { get; set; } = string.Empty;
    public string Street { get; set; } = string.Empty;
    public string Street_number { get; set; } = string.Empty;

    public override string ToString()
    {
        return $"{Province}{City}{District}{Street}{Street_number}";
    }
}