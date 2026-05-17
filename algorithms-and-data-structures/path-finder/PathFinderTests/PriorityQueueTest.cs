using System.Drawing;
namespace PathFinderTests;
using PathFinder;

public class PriorityQueueTests
{
    /// <summary>
    /// Replace this type with your custom PriorityQueue
    /// </summary>
    private CustomPriorityQueue _points = new CustomPriorityQueue();
    
    [SetUp]
    public void Setup()
    {
        // also do it here
        _points = new CustomPriorityQueue();
        _points.Enqueue(1, new PathFinder.MapGeneration.Point(1, 1));
        _points.Enqueue(4, new PathFinder.MapGeneration.Point(2, 2));
        _points.Enqueue(3, new PathFinder.MapGeneration.Point(3, 3));
        _points.Enqueue(2, new PathFinder.MapGeneration.Point(4, 4));
    }

    [Test]
    public void TestCount()
    {
        Assert.That(_points.Count, Is.EqualTo(4));
    }

    [Test]
    public void TestDequeued()
    {
        Assert.That(_points.Dequeue().Item2, Is.EqualTo(new PathFinder.MapGeneration.Point(1, 1)));
    }
    
    [Test]
    public void TestEnqueue()
    {
        _points.Enqueue(2, new PathFinder.MapGeneration.Point(5,5));
        _points.Dequeue();
        Assert.That(_points.Dequeue().Item2, Is.EqualTo(new PathFinder.MapGeneration.Point(5, 5)));
    }
    
    [Test]
    public void TestPeek()
    {
        _points.Enqueue(1, new PathFinder.MapGeneration.Point(5,5));
        _points.Dequeue();
        Assert.That(_points.Peek().Item2, Is.EqualTo(new PathFinder.MapGeneration.Point(5, 5)));
    }
}
