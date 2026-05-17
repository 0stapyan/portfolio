using PathFinder;
using PathFinder.MapGeneration;

namespace PathFinderTests;

public class PathFinderTrafficTests
{
    private readonly Point _start = new(0, 0);
    private readonly Point _destination = new(18, 18);
    private string[,]? _map;

    [SetUp]
    public void Setup()
    {
        var optionsToGenerate = new MapGeneratorOptions()
        {
            Height = 20,
            Width = 20,
            Noise = 0.2f,
            Seed = 1000,
            AddTraffic = true,
            TrafficSeed = 13
        };

        var generator = new MapGenerator(optionsToGenerate);
        _map = generator.Generate();
    }

    [Test]
    [Category("WithTraffic")]
    public void TestDijkstraWeighted()
    {
        var d = new Dijkstra();
        var (shortestPath, nodesVisited, time) = d.FindPathUsingDijkstra(_map!, _start, _destination);

        var expectedPath = Paths.DijkstraPathWeighted;

        Assert.That(nodesVisited, Is.EqualTo(216));
        Assert.That(shortestPath, Is.EqualTo(expectedPath).AsCollection);
    }
    
    [Test]
    [Category("WithTraffic")]
    public void TestAStarWeighted()
    {
        var a = new A_();
        var (shortestPath, nodesVisited, time, visitedList) = a.FindPathUsingA(_map!, _start, _destination);
    
        var expectedPath = Paths.AStarPathWeighted;
    
        Assert.That(nodesVisited, Is.EqualTo(198));
        Assert.That(shortestPath, Is.EqualTo(expectedPath).AsCollection);
    }
}