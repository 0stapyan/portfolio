using System.Globalization;

namespace SpatialSearch;

public class Point
{
    public readonly double Latitude;
    public readonly double Longitude;
    public readonly string Activity;
    public readonly string TypeOfFacility;
    public readonly string Facility;

    public Point(List<string> entries)
    {
        Latitude = double.Parse(entries[0], CultureInfo.InvariantCulture);
        Longitude = double.Parse(entries[1], CultureInfo.InvariantCulture);
        Activity = entries[2];
        TypeOfFacility = entries[4];
        Facility = entries[5];
    }

    public Point(double latitude, double longitude, string activity, string typeoffacility, string facility)
    {
        Latitude = latitude;
        Longitude = longitude;
        Activity = activity;
        TypeOfFacility = typeoffacility;
        Facility = facility;
    }

    public override string ToString()
    {
        return $"{Activity}, {TypeOfFacility}, {Facility}: {Latitude} - {Longitude}";
    }

    public override bool Equals(object? obj)
    {
        if (obj is not Point other) return false;
        return Math.Abs(Latitude - other.Latitude) < 0.00001 &&
               Math.Abs(Longitude - other.Longitude) < 0.00001;
    }

    public override int GetHashCode()
    {
        return Latitude.GetHashCode() ^ Longitude.GetHashCode();
    }
}