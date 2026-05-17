using System.Collections;
using PathFinder;
using PathFinder.MapGeneration;
using Point = PathFinder.MapGeneration.Point;

var optionsToGenerate = new MapGeneratorOptions()
{
    Height = 40,
    Width = 150,
    Noise = 0.5f,
    AddTraffic = true,
};
var generator = new MapGenerator(optionsToGenerate);
string[,]? map = generator.Generate();

Point startPoint = Point.GeneratePoint(optionsToGenerate);
Point endPoint = Point.GeneratePoint(optionsToGenerate);

var dijkstraResult = new DijkstraWithDiagonals().FindFastestTimePathUsingDiagonals(map, startPoint, endPoint);
var aStarResult = new AStarWithDiagonals().FindFastestTimePathUsingDiagonals(map, startPoint, endPoint);


Console.WriteLine("Кількість відвіданих вузлів (Dijkstra diagonals): " + dijkstraResult.Item2);
Console.WriteLine("Кількість відвіданих вузлів (A* diagonals): " + aStarResult.Item2);
Console.WriteLine();

Console.WriteLine("Час проходження (Dijkstra diagonals): " + dijkstraResult.Item3 + " секунд");
Console.WriteLine("Час проходження (A* diagonals): " + aStarResult.Item3 + " секунд");

var printer = new MapPrinter();

Console.WriteLine("\nШлях за алгоритмом Dijkstra diagonals:");
printer.Print(map, startPoint, endPoint, dijkstraResult.Item1, optionsToGenerate, dijkstraResult.Item3);

Console.WriteLine("\nШлях за алгоритмом A* diagonals:");
printer.Print(map, startPoint, endPoint, aStarResult.Item1, optionsToGenerate, aStarResult.Item3);


// Visualizer visualizer = new Visualizer();
// visualizer.Print(map, startPoint, endPoint, diagonalResult.Item4, diagonalResult.Item1);