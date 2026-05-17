using PathFinder.MapGeneration;

namespace PathFinder;
using System;
using System.Collections.Generic;

public class CustomPriorityQueue
{
    public List<(float, Point)> list = new();
    public int Count { get { return list.Count; } }
    
    public void Enqueue(float priority, Point point)
    {
        list.Add((priority, point));
        int i = Count - 1;

        while (i > 0)
        {
            int p = (i - 1) / 2;
            if (list[p].Item1 <= priority) break;

            list[i] = list[p];
            i = p;
        }

        if (Count > 0) list[i] = (priority, point);
    }

    public (float, Point) Dequeue()
    {
        var res = Peek();
        
        (float root, Point point) = list[Count - 1];
        list.RemoveAt(Count - 1);

        int i = 0;
        while (i * 2 + 1 < Count)
        {
            int a = i * 2 + 1;
            int b = i * 2 + 2;
            
            int c;
            
            if (b < Count && list[b].Item1 < list[a].Item1)
            {
                c = b;
            }
            else
            {
                c = a;
            }

            if (list[c].Item1 >= root) break;
            list[i] = list[c];
            i = c;
        }

        if (Count > 0) list[i] = (root, point);
        return res;
    }

    public (float, Point) Peek()
    {
        if (Count == 0)
        {
            throw new ArgumentException();
        }
        return list[0];
    }
}