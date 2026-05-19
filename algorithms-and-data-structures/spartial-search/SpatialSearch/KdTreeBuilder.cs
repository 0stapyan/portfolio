namespace SpatialSearch;

public static class KdTreeBuilder
{
    private const int LeafSize = 10;

    public static KdNode Build(List<Point> points, bool splitByLatitude = true)
    {
        if (points.Count <= LeafSize)
        {
            return new KdNode
            {
                LeafPoints = points
            };
        }

        var sorted = points.OrderBy(p => splitByLatitude ? p.Latitude : p.Longitude).ToList();
        int medianIndex = sorted.Count / 2;
        var medianPoint = sorted[medianIndex];

        var leftPoints = sorted.Take(medianIndex).ToList();
        var rightPoints = sorted.Skip(medianIndex + 1).ToList();

        return new KdNode
        {
            Point = medianPoint,
            SplitByLatitude = splitByLatitude,
            Left = Build(leftPoints, !splitByLatitude),
            Right = Build(rightPoints, !splitByLatitude)
        };
    }
}