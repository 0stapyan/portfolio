using PathFinder.MapGeneration;

namespace PathFinder;

public class CustomQueue
{
    private List<Point> queue = new List<Point>();
    public Point Dequeue()
    {
        Point res = queue[0];
        queue.RemoveAt(0);
        return res;
    }

    public void Enqueue(Point point)
    {
        queue.Add(point);
    }

    public int Count()
    {
        return queue.Count;
    }
}