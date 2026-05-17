namespace Calculator;

public class MyList<type>
{
    private type[] items;
    private int count;

    public MyList()
    {
        items = new type[4];
        count = 0;
    }
    public void ShowList()
    {
        for (int i = 0; i < count; i++)
        {
            Console.Write(items[i] + (i < count - 1 ? ", " : ""));
        }
    }
    public void Add(type item)
    {
        if (count == items.Length)
        {
            Array.Resize(ref items, items.Length * 2);
        }
        items[count++] = item;
    }

    public type Get(int index)
    {
        if (index < 0 || index >= count)
            throw new IndexOutOfRangeException();
        return items[index];
    }

    public int Count => count;
}