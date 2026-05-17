namespace Calculator;

public class MyStack<T>
{
    private T[] Data;
    private int LocalSize;

    public MyStack(int size = 4)
    {
        Data = new T[size];
        LocalSize = 0;
    }

    public void Push(T item)
    {
        if (LocalSize == Data.Length)
        {
            Array.Resize(ref Data, Data.Length * 2);
        }
        Data[LocalSize++] = item;
    }

    public T Pop()
    {
        if (IsEmpty())
        {
            throw new InvalidOperationException("Stack is empty");
        }
        return Data[--LocalSize];
    }

    public T Top()
    {
        if (IsEmpty())
        {
            throw new InvalidOperationException("Stack is empty");
        }
        return Data[LocalSize - 1];
    }

    public bool IsEmpty()
    {
        return LocalSize == 0;
    }

    public int GetSize()
    {
        return LocalSize;
    }
}