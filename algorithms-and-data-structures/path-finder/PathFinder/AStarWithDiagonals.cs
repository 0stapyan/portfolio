using System.Net;
using PathFinder.MapGeneration;

namespace PathFinder;

public class AStarWithDiagonals
{
    public (List<Point>, int, float, List<Point>) FindFastestTimePathUsingDiagonals(string[,] map, Point start,
        Point destination)
    {
        map[start.Column, start.Row] = " ";
        map[destination.Column, destination.Row] = " ";

        Dictionary<Point, Point> origins = new Dictionary<Point, Point>();
        Dictionary<Point, float> times = new Dictionary<Point, float>();

        for (int i = 0; i < map.GetLength(0); i++)
        {
            for (int j = 0; j < map.GetLength(1); j++)
            {
                int number;
                if (map[i, j] == " ")
                {
                    times[new Point(i, j)] = int.MaxValue;
                }
                else if (int.TryParse(map[i, j], out number))
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

                float octileDistance = Octile(neighbour, destination);
                if (times[next.Item2] + GetTime(next.Item2, neighbour, map) < times[neighbour])
                {
                    times[neighbour] = times[next.Item2] + GetTime(next.Item2, neighbour, map);
                    origins[neighbour] = next.Item2;
                    queue.Enqueue(times[neighbour] + octileDistance*60, neighbour);
                }
            }
        }


        List<Point> path = GetShortestPath(origins, start, destination);
        float time = GetTotalTime(path, map);
        return (path, visited.Count, time, visited);
    }

    List<Point> GetNeighbours(int column, int row, string[,] maze)
    {
        var result = new List<Point>();
        TryAddWithOffset(1, 0);
        TryAddWithOffset(-1, 0);
        TryAddWithOffset(0, 1);
        TryAddWithOffset(0, -1);
        TryAddWithOffset(1, 1);
        TryAddWithOffset(1, -1);
        TryAddWithOffset(-1, 1);
        TryAddWithOffset(-1, -1);
        return result;

        void TryAddWithOffset(int offsetColumn, int offsetRow)
        {
            var newRow = row + offsetRow;
            var newColumn = column + offsetColumn;
            if (newColumn >= 0 && newRow >= 0 && newColumn < maze.GetLength(0) && newRow < maze.GetLength(1) &&
                maze[newColumn, newRow] != "█")
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

    public float GetTotalTime(List<Point> path, string[,] map)
    {
        float time = 0;
        for (int i = 0; i < path.Count; i++)
        {
            int value = GetDistance(path[i], map);

            int speed = 60 - (value - 1) * 6;

            if (i == 0)
            {
                time += 3600 / speed;
            }
            else
            {
                if (DiagonalCheck(path[i - 1], path[i], map))
                {
                    time += (float)(Math.Sqrt(2) * 3600 / (float)speed);
                    ;
                }
                else
                {
                    time += 3600 / speed;
                }
            }
        }

        return time;
    }

    public float GetTime(Point point, Point neighbour, string[,] map)
    {
        if (DiagonalCheck(point, neighbour, map))
        {
            return (float)(Math.Sqrt(2) * 3600 / (60 - (GetDistance(neighbour, map) - 1) * 6));
        }

        return 3600 / (60 - (GetDistance(neighbour, map) - 1) * 6);
    }

    public bool DiagonalCheck(Point point, Point neighbour, string[,] map)
    {
        int distance = Math.Abs(point.Row - neighbour.Row) + Math.Abs(point.Column - neighbour.Column);

        if (distance == 2)
        {
            return true;
        }

        return false;
    }

    public float Octile(Point currPoint, Point endPoint)
    {
        float D1 = 1f;
        float D2 = (float)Math.Sqrt(2);
        float dx = Math.Abs(currPoint.Column - endPoint.Column);
        float dy = Math.Abs(currPoint.Row - endPoint.Row);
        return D1 * (dx + dy) + (D2 - 2 * D1) * Math.Min(dx, dy);
    }
}
