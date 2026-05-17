namespace HuffmanCodes;

public class SymbolFrequency
{
    public static Dictionary<char, int> GetSymbolFrequency(string filePath)
    {
        var frequency = new Dictionary<char, int>();
        var allText = File.ReadAllText(filePath);

        foreach (var symbol in allText)
        {
            if (frequency.TryGetValue(symbol, out var count))
            {
                frequency[symbol] = count + 1;
            }
            else
            {
                frequency[symbol] = 1;
            }
        }

        return frequency;
    }
}