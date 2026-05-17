namespace Calculator;

public class Tokenizer
{
    private MyQueue<string> Tokens;
    private string Expression;

    public Tokenizer(string expression)
    {
        Expression = expression;
        Tokens = new MyQueue<string>();
    }

    public MyQueue<string> ShowTokens()
    {
        Tokens.ShowQueue();
        return Tokens;
    }

    public void Tokenize()
    {
        string buffer = "";
        foreach (char c in Expression + " ")
        {
            if (char.IsDigit(c) || c == '.')
            {
                buffer += c;
            }
            else if (char.IsWhiteSpace(c))
            {
                if (buffer.Length > 0)
                {
                    Tokens.Enqueue(buffer);
                    buffer = "";
                }
            }
            else
            {
                if (buffer.Length > 0)
                {
                    Tokens.Enqueue(buffer);
                    buffer = "";
                }
                Tokens.Enqueue(c.ToString());
            }
        }
    }
}