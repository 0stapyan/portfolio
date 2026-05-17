using System.Net;
using PathFinder.MapGeneration;

namespace PathFinder;

public class AStarShortestTime
{
    public (List<Point>, int, int, List<Point>) FindFastestTimePathUsingA(string[,] map, Point start, Point destination)
    {
        map[start.Column, start.Row] = " ";
        map[destination.Column, destination.Row] = " ";
        
        Dictionary<Point, Point> origins = new Dictionary<Point, Point>();
        Dictionary<Point, int> times = new Dictionary<Point, int>();

        for (int i = 0; i < map.GetLength(0); i++)
        {
            for (int j = 0; j < map.GetLength(1); j++)
            {
                int number;
                if (map[i, j] == " ")
                {
                    times[new Point(i, j)] = int.MaxValue;
                }
                else if (int.TryParse(map[i,j], out number))
                {
                    times[new Point(i, j)] = int.MaxValue;
                }
            }
        }

        times[start] = 0;
        origins[start] = start;
        
        
        var visited = new List<Point>();
        var queue = new CustomPriorityQueue();
        
        queue.Enqueue(0, start);
        
        
        while (queue.Count > 0)
        {
            (float, Point) next = queue.Dequeue();
            if (visited.Contains(next.Item2))
            {
                continue;
            }
            if (next.Item2.Equals(destination))
            {
                break;
            }
            visited.Add(next.Item2);
            var neighbours = GetNeighbours(next.Item2.Column, next.Item2.Row, map);
            
            foreach (Point neighbour in neighbours)
            {
                if (visited.Contains(neighbour))
                {
                    continue;
                }
                int manhattanDistance = Math.Abs(destination.Column - neighbour.Column) + Math.Abs(destination.Row - neighbour.Row);
                if (times[next.Item2] + GetTime(neighbour, map) < times[neighbour])
                {
                    times[neighbour] = times[next.Item2] + GetTime(neighbour, map);
                    origins[neighbour] = next.Item2;
                    queue.Enqueue(times[neighbour] + manhattanDistance*61, neighbour);
                }
            }
        }
        

        List <Point> path = GetShortestPath(origins, start, destination);
        int time = GetTotalTime(path, map);
        return (path, visited.Count, time, visited);
    }
    
    List<Point> GetNeighbours(int column, int row, string[,] maze)
    {
        var result = new List<Point>();
        TryAddWithOffset(1, 0);
        TryAddWithOffset(-1, 0);
        TryAddWithOffset(0, 1);
        TryAddWithOffset(0, -1);
        return result;

        void TryAddWithOffset(int offsetColumn, int offsetRow)
        {
            var newRow = row + offsetRow;
            var newColumn = column + offsetColumn;
            if (newColumn >= 0 && newRow >= 0 && newColumn < maze.GetLength(0) && newRow < maze.GetLength(1) && maze[newColumn, newRow] != "█")
            {
                result.Add(new Point(newColumn, newRow));
            }
        }
    }

    List<Point> GetShortestPath(Dictionary<Point, Point> origins, Point start, Point destination)
    {
        List<Point> path = new List<Point>();
        Point currPoint = destination;

        while (currPoint.Column != start.Column || currPoint.Row != start.Row)
        {
            path.Add(currPoint);
            currPoint = origins[currPoint];
        }

        return path;
    }

    int GetDistance(Point point, string[,] map)
    {
        int value;
        if (int.TryParse(map[point.Column, point.Row], out value))
        {
            return value;
        }

        return 1;
    }

    public int GetTotalTime(List<Point> path, string[,] map)
    {
        int time = 0;
        foreach (var point in path)
        {
            int value = GetDistance(point, map);
            
            int speed = 60 - (value - 1) * 6;

            time += 3600 / speed;
        }

        return time;
    }
    
    public int GetTime(Point point, string[,] map)
    {
        return 3600 / (60 - (GetDistance(point, map) - 1) * 6);
    }
}
