namespace Calculator;

public class ShuntingYard
{
    private MyQueue<string> output;
    private MyStack<string> operators;

    public ShuntingYard()
    {
        output = new MyQueue<string>();
        operators = new MyStack<string>(100);
    }

    private int GetPrecedence(string op)
    {
        return op switch
        {
            "+" or "-" => 1,
            "*" or "/" => 2,
            "^" => 3,
            "$" => 4,
            _ => 0
        };
    }

    public MyQueue<string> ConvertToRPN(MyQueue<string> tokens)
    {
        while (!tokens.IsEmpty())
        {
            string token = tokens.Dequeue();

            if (int.TryParse(token, out _))
            {
                output.Enqueue(token);
            }
            else if ("+-*/^$".Contains(token))
            {
                while (!operators.IsEmpty() &&
                       GetPrecedence(operators.Top()) >= GetPrecedence(token))
                {
                    output.Enqueue(operators.Pop());
                }
                operators.Push(token);
            }
            else if (token == "(")
            {
                operators.Push(token);
            }
            else if (token == ")")
            {
                while (!operators.IsEmpty() && operators.Top() != "(")
                {
                    output.Enqueue(operators.Pop());
                }
                if (!operators.IsEmpty())
                {
                    operators.Pop(); // Видаляємо відкриту дужку
                }
            }
        }

        while (!operators.IsEmpty())
        {
            output.Enqueue(operators.Pop());
        }

        return output;
    }
}