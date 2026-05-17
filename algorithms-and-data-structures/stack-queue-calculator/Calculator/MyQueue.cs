namespace Calculator;

public class MyQueue<type>
{
    private type[] items;
    private int front;
    private int rear;
    private int count;

    public MyQueue()
    {
        items = new type[4];
        front = 0;
        rear = -1;
        count = 0;
    }

    public void Enqueue(type item)
    {
        if (count == items.Length)
        {
            Array.Resize(ref items, items.Length * 2);
        }
        rear = (rear + 1) % items.Length;
        items[rear] = item;
        count++;
    }

    public type Dequeue()
    {
        if (IsEmpty())
            throw new InvalidOperationException("Queue is empty");

        type item = items[front];
        front = (front + 1) % items.Length;
        count--;
        return item;
    }

    public type Peek()
    {
        if (IsEmpty())
            throw new InvalidOperationException("Queue is empty");
        return items[front];
    }

    public bool IsEmpty()
    {
        return count == 0;
    }

    public int Count => count;

    public void ShowQueue()
    {
        for (int i = 0; i < count; i++)
        {
            Console.Write(items[(front + i) % items.Length] + (i < count - 1 ? ", " : ""));
        }
    }
}