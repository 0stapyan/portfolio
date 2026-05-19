using System.Diagnostics;
using System.Drawing;
using System.Globalization;

namespace SpatialSearch;

public static class SpatialSearch
{
    public static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.InputEncoding = System.Text.Encoding.UTF8;
        Console.WriteLine("Enter the radius of circle (in km): ");
        double rad = double.Parse(Console.ReadLine()!);

        Console.WriteLine("Enter the latitude of circle center: ");
        double lat = double.Parse(Console.ReadLine()!);

        Console.WriteLine("Enter the longitude of circle center: ");
        double lon = double.Parse(Console.ReadLine()!);
        
        List<Point> pointPOIList = new();
        string filePath = "/Users/admin/Documents/unik/it/ads/assignment-5-spatial-search-asd1-aboichuk-oturash/SpatialSearch/Data/ukraine_poi.csv";
        string filePath2 = "C:/Users/aboic/RiderProjects/assignment-5-spatial-search-asd1-aboichuk-oturash/SpatialSearch/Data/ukraine_poi.csv";
        List<string> POIlist = File.ReadAllLines(filePath2).ToList();
        var cultureparse = new CultureInfo("fr-FR");
        

        foreach (string item in POIlist)
        {
            var splitteditem = item.Split(";");
            double latitude = double.Parse(splitteditem[0], cultureparse);
            double longitude = double.Parse(splitteditem[1], cultureparse);
            pointPOIList.Add(new Point(latitude, longitude, splitteditem[2], splitteditem[4], splitteditem[5]));
        }
        var lowestLong = pointPOIList.Min(p => p.Longitude);
        var highestLong = pointPOIList.Max(p => p.Longitude);
        var lowestLat = pointPOIList.Min(p => p.Latitude);
        var highestLat = pointPOIList.Max(p => p.Latitude);
        
        var diffLong = highestLong - lowestLong;
        var diffLat = highestLat - lowestLat;

        Random rnd = new();
        
        
        int width = 500;
        int height = 500;
        Bitmap bmp = new Bitmap(width, height);
        
        
        Stopwatch treeWatch = new();
        treeWatch.Start();
        var kdTree = KdTreeBuilder.Build(pointPOIList);
        treeWatch.Stop();
        Console.WriteLine($"\nKd-дерево побудовано за: {treeWatch.ElapsedMilliseconds} мс");
        List<Tuple<KdNode, double, double, double>> inputtuple = new();
        for (int i = 0; i < 1000; i++)
        {
            double randlat = lowestLat + (rnd.NextDouble() * (highestLat - lowestLat));
            double randlong = lowestLong + (rnd.NextDouble() * (highestLong - lowestLong));
            double randrad = rnd.Next(0, 201);
            inputtuple.Add(new Tuple<KdNode, double, double, double>(kdTree, randlat, randlong, randrad));
        }

        Stopwatch bruteWatch = new();
        bruteWatch.Start();

        List<Point> bruteResults = new();
        foreach (var tuple in inputtuple)
        {
            foreach (var point in pointPOIList)
            {
                if (Haversine(point, tuple.Item2, tuple.Item3) < tuple.Item4)
                {
                    bruteResults.Add(point);
                }
            }
        }
        

        bruteWatch.Stop();

        //Console.WriteLine("\n--- Результати лінійного пошуку ---");
        //foreach (var item in bruteResults)
        //{
            //Console.WriteLine(item.ToString());
        //}

        Console.WriteLine($"\nЛінійний пошук зайняв: {bruteWatch.ElapsedMilliseconds} мс");
        
        
        Stopwatch kdWatch = new();
        kdWatch.Start();
        var alotofkdresult = new List<List<Point>>();
        foreach (var tuple in inputtuple)
        {

            var res = KdSearch.Search(tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4);
            alotofkdresult.Add(res);
        }
        var kdResults = KdSearch.Search(kdTree, lat, lon, rad);
        
        kdWatch.Stop();

        //Console.WriteLine("\n--- Результати пошуку в K-d дереві ---");
        //foreach (var item in alotofkdresult[999])
        //{
                //Console.WriteLine(item.ToString());
            
        //}

        Console.WriteLine($"\nПошук у K-d дереві зайняв: {kdWatch.ElapsedMilliseconds} мс");
        using (Graphics g = Graphics.FromImage(bmp))
        {
            g.Clear(Color.White);
            Brush brBl = new SolidBrush(Color.Black);
            foreach (var point in pointPOIList)
            {
                int placeLong = Convert.ToInt32((point.Longitude - lowestLong)/diffLong*width);
                int placeLat = Convert.ToInt32((point.Latitude - lowestLat)/diffLat*height);
                
                g.FillEllipse(brBl, placeLong, placeLat, 2,2);
            }
            double radLatDegrees = rad / 111;
            double radLongDegrees = rad / (111 * Math.Cos(lat * Math.PI / 180));
            int Radx = Convert.ToInt32(radLongDegrees / diffLong * width);
            int RadY = Convert.ToInt32(radLatDegrees / diffLat * height);
            int circleCenterX = Convert.ToInt32((lon - lowestLong) / diffLong * width);
            int circleCenterY = Convert.ToInt32((lat - lowestLat) / diffLat * height);
            Pen penR = new Pen(Color.Red, 2);
            g.DrawEllipse(penR, circleCenterX - Radx, circleCenterY - RadY, Radx * 2, RadY * 2);

            Brush brGr = new SolidBrush(Color.SpringGreen);
            Brush brPu = new SolidBrush(Color.MediumPurple);
            foreach (var point in kdResults)
            {
                int placeLong = Convert.ToInt32((point.Longitude - lowestLong)/diffLong*width);
                int placeLat = Convert.ToInt32((point.Latitude - lowestLat)/diffLat*height);
                g.FillEllipse(brGr, placeLong, placeLat, 2,2);
            }
            foreach (var point in bruteResults)
            {
                int placeLong = Convert.ToInt32((point.Longitude - lowestLong)/diffLong*width);
                int placeLat = Convert.ToInt32((point.Latitude - lowestLat)/diffLat*height);
                g.FillEllipse(brPu, placeLong, placeLat, 2,2); 
            }
            

        }
        bmp.Save("mapdots.png");
    }

    public static double Haversine(Point point, double latitudeofcircle, double longitudeofcircle)
    {
        double R = 6371.0;

        double dLat = DegreesToRadians(point.Latitude - latitudeofcircle);
        double dLon = DegreesToRadians(point.Longitude - longitudeofcircle);

        double lat1Rad = DegreesToRadians(point.Latitude);
        double lat2Rad = DegreesToRadians(latitudeofcircle);

        double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                   Math.Cos(lat1Rad) * Math.Cos(lat2Rad) *
                   Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        return R * c;
    }

    public static double DegreesToRadians(double deg)
    {
        return deg * Math.PI / 180.0;
    }
}
