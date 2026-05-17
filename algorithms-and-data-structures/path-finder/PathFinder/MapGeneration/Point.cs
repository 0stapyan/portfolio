namespace PathFinder.MapGeneration;

public struct Point
{
    public int Column { get; }
    public int Row { get; }

    public Point(int column, int row)
    {
        Column = column;
        Row = row;
    }
    
    public static Point GeneratePoint(MapGeneratorOptions options)
    {
        Random rnd = new Random();
        
        int column = rnd.Next(0, options.Width) - 1;
        int row = rnd.Next(0,options.Height - 1);

        return new Point(column, row);
    }
}