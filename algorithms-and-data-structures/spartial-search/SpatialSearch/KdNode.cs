namespace SpatialSearch;

public class KdNode
{
    public Point? Point { get; set; }
    public KdNode? Left { get; set; }
    public KdNode? Right { get; set; }
    public bool SplitByLatitude { get; set; }

    public List<Point>? LeafPoints { get; set; }

    public bool IsLeaf => LeafPoints != null;
}