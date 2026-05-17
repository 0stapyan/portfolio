namespace PathFinder.MapGeneration;

public class MapPrinter
{
    public void Print(string[,] maze, MapGeneratorOptions options, Point startPoint, Point endPoint, List<System.Drawing.Point> shortestPath, float time)
    {
        PrintMap(maze, startPoint, endPoint, shortestPath.Select(p => new Point(p.X, p.Y)).ToList(), options);
        PrintTime(time);
    }

    public void Print(string[,] maze, Point startPoint, Point endPoint, List<Point> shortestPath, MapGeneratorOptions options, float time)
    {
        PrintMap(maze, startPoint, endPoint, shortestPath, options);
        PrintTime(time);
    }

    private void PrintMap(string[,] maze, Point startPoint, Point endPoint, List<Point> shortestPath, MapGeneratorOptions options)
    {
        PrintTopLine(maze);
        for (var row = 0; row < maze.GetLength(1); row++)
        {
            Console.Write($"{row}\t");
            for (var column = 0; column < maze.GetLength(0); column++)
            {
                var currentPoint = new Point(column, row);
                if (currentPoint.Equals(startPoint))
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.Write("A");
                    Console.ResetColor();
                }
                else if (currentPoint.Equals(endPoint))
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.Write("B");
                    Console.ResetColor();
                }
                else if (shortestPath.Contains(currentPoint))
                {
                    Console.BackgroundColor = ConsoleColor.Magenta;
                    if (options.AddTraffic)
                    {
                        Console.ResetColor();
                        Console.ForegroundColor = ConsoleColor.Magenta;
                    }
                    Console.Write(maze[column, row]);
                    Console.ResetColor();
                }
                else
                {
                    Console.Write(maze[column, row]);
                }
            }
            Console.WriteLine();
        }
    }

    private void PrintTime(float time)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"\nЧас проходження шляху: {time} секунд");
        Console.ResetColor();
    }

    private void PrintTopLine(string[,] maze)
    {
        Console.Write($" \t");
        for (int i = 0; i < maze.GetLength(0); i++)
        {
            Console.Write(i % 10 == 0 ? i / 10 : " ");
        }

        Console.Write($"\n \t");
        for (int i = 0; i < maze.GetLength(0); i++)
        {
            Console.Write(i % 10);
        }

        Console.WriteLine("\n");
    }
}