using PathFinder;
using PathFinder.MapGeneration;


namespace PathFinderTests;

public class PathFinderTests
{
    private readonly Point _start = new(1, 1);
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
            Seed = 1000
        };

        var generator = new MapGenerator(optionsToGenerate);
        _map = generator.Generate();
    }

    [Test]
    [Category("WithoutTraffic")]
    public void TestBreadthFirstSearch()
    {

        BreadthFirstSearch bfs = new BreadthFirstSearch();
        var (shortestPath, nodesVisited, time) = bfs.FindPath(_map!, _start, _destination);

        var expectedPath = Paths.BfsPath;

        Assert.That(nodesVisited, Is.EqualTo(239));
        Assert.That(shortestPath, Is.EqualTo(expectedPath).AsCollection);
    }

    [Test]
    [Category("WithoutTraffic")]
    public void TestDijkstraUnweighted()
    {
        var dijkstra = new Dijkstra();
        var (shortestPath, nodesVisited, time) = dijkstra.FindPathUsingDijkstra(_map!, _start, _destination);

        var expectedPath = Paths.DijkstraPathUnweighted;

        Assert.That(nodesVisited, Is.EqualTo(240));
        Assert.That(shortestPath, Is.EqualTo(expectedPath).AsCollection);
    }

    [Test]
    [Category("WithoutTraffic")]
    public void TestAStarUnweighted()
    {
        // Replace this with your actual implementation of AStar
        var aStar = new A_();
        var (shortestPath, nodesVisited, time, visitedList) = aStar.FindPathUsingA(_map!, _start, _destination);

        var expectedPath = Paths.AStarPathUnweighted;

        Assert.That(nodesVisited, Is.EqualTo(111));
        Assert.That(shortestPath, Is.EqualTo(expectedPath).AsCollection);
    }
}