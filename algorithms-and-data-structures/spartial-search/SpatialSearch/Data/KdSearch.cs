namespace SpatialSearch;

public static class KdSearch
{
    public static List<Point> Search(KdNode node, double latCenter, double lonCenter, double radiusKm)
    {
        List<Point> result = new();
        SearchRecursive(node, latCenter, lonCenter, radiusKm, result);
        return result;
    }

    private static void SearchRecursive(KdNode node, double latCenter, double lonCenter, double radiusKm, List<Point> result)
    {
        if (node.IsLeaf && node.LeafPoints != null)
        {
            foreach (var point in node.LeafPoints)
            {
                if (SpatialSearch.Haversine(point, latCenter, lonCenter) <= radiusKm)
                {
                    result.Add(point);
                }
            }
            return;
        }

        if (node.Point == null) return;

        double coordToCompare = node.SplitByLatitude ? node.Point.Latitude : node.Point.Longitude;
        double centerCoord = node.SplitByLatitude ? latCenter : lonCenter;
        
        if (centerCoord - radiusKmToDegrees(radiusKm) <= coordToCompare && node.Left != null)
            SearchRecursive(node.Left, latCenter, lonCenter, radiusKm, result);

        if (centerCoord + radiusKmToDegrees(radiusKm) >= coordToCompare && node.Right != null)
            SearchRecursive(node.Right, latCenter, lonCenter, radiusKm, result);
    }

    private static double radiusKmToDegrees(double km)
    {
        return km / 111.0;
    }
}