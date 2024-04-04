namespace Known.BMap;

public class SearchResult
{
    public string Uid { get; set; }
    public int Type { get; set; }
    public string Title { get; set; }
    public string Address { get; set; }
    public string Province { get; set; }
    public string City { get; set; }
    public string Url { get; set; }
    public string DetailUrl { get; set; }
    public string PhoneNumber { get; set; }
    public string Postcode { get; set; }
    public string Src_type { get; set; }
    public bool IsAccurate { get; set; }
    public Point Point { get; set; }
}

public class Point
{
    public double Lng { get; set; }
    public double Lat { get; set; }
}