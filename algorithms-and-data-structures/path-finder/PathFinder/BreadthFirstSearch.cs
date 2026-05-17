using PathFinder.MapGeneration;

namespace PathFinder;

public class BreadthFirstSearch
{
    public (List<Point>, int, int) FindPath(string[,] map, Point start, Point destination)
    {
        map[start.Column, start.Row] = " ";
        map[destination.Column, destination.Row] = " ";
        
        Dictionary<Point, Point> origins = new Dictionary<Point, Point>();
        Dictionary<Point, int> distances = new Dictionary<Point, int>();
        
        var visited = new List<Point>();
        var queue = new CustomQueue();
        
        visited.Add(start);
        queue.Enqueue(start);
        origins[start] = start;
        
        bool end = false;
        
        while (queue.Count() > 0 && !end)
        {
            var next = queue.Dequeue();
            var neighbours = GetNeighbours(next.Column, next.Row, map);
            foreach (var neighbour in neighbours)
            {
                if (!visited.Contains(neighbour))
                {
                    origins[neighbour] = next;
                    visited.Add(neighbour);
                    if ((neighbour.Row == destination.Row) && (neighbour.Column == destination.Column))
                    {
                        end = true;
                        break;
                    }
                    queue.Enqueue(neighbour);
                }
            }
        }

        List <Point> path = GetShortestPath(origins, start, destination);
        int time = GetTime(path, map);
        return (path, visited.Count, time);
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

    public int GetTime(List<Point> path, string[,] map)
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
}