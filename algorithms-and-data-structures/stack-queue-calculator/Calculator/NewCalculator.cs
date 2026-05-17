namespace Calculator;

public class NewCalculator
{
    private MyStack<int> Stack;
    private MyQueue<string> Tokens;
    public int ans;

    public NewCalculator(int size, MyQueue<string> tokens)
    {
        Tokens = tokens;
        Stack = new MyStack<int>(size);
    }

    public int Operation(int operand1, int operand2, string operation)
    {
        return operation switch
        {
            "+" => operand1 + operand2,
            "-" => operand1 - operand2,
            "*" => operand1 * operand2,
            "/" => operand1 / operand2,
            "^" => (int)Math.Pow(operand1, operand2),
            "$" => (int)Math.Min(operand1, operand2),
            _ => throw new InvalidOperationException("Unknown operator: " + operation)
        };
    }

    public int Calculate()
    {
        int size = Tokens.Count;
        for (int i = 0; i < size; i++)
        {
            string peek = Tokens.Peek();
            if ("+-*/^$".Contains(peek)) // Додано ^
            {
                int Operand_2 = Stack.Pop();
                int Operand_1 = Stack.Pop();
                int result = Operation(Operand_1, Operand_2, peek);
                Stack.Push(result);
            }
            else
            {
                Stack.Push(int.Parse(peek));
            }
            Tokens.Dequeue();
        }

        ans = Stack.Pop();
        return ans;
    }
}