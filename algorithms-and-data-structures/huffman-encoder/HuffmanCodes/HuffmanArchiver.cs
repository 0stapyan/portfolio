using System.Text;

namespace HuffmanCodes;

public static class HuffmanArchiver
{
    public static string EncodeText(string text, Dictionary<char, string> codes)
    {
        var sb = new StringBuilder();
        foreach (var c in text)
        {
            sb.Append(codes[c]);
        }
        return sb.ToString();
    }

    public static void SaveEncodedText(string encodedText, string filePath)
    {
        File.WriteAllText(filePath, encodedText);
    }

    public static void SaveCodeTable(Dictionary<char, string> codes, string filePath)
    {
        var lines = codes.Select(kvp => $"{kvp.Key}:{kvp.Value}");
        File.WriteAllLines(filePath, lines);
    }

    public static Dictionary<char, string> LoadCodeTable(string filePath)
    {
        var lines = File.ReadAllLines(filePath);
        var codes = new Dictionary<char, string>();
        foreach (var line in lines)
        {
            var separatorIndex = line.IndexOf(':');
            var symbol = line.Substring(0, separatorIndex)[0];
            var code = line.Substring(separatorIndex + 1);
            codes[symbol] = code;
        }
        return codes;
    }

    public static string DecodeText(string encodedText, HuffmanNode root)
    {
        var result = new StringBuilder();
        var current = root;

        foreach (var bit in encodedText)
        {
            if (bit == '0')
            {
                if (current.Left == null)
                    throw new InvalidOperationException("Невірний формат дерева Хафмана.");
                current = current.Left;
            }
            else
            {
                if (current.Right == null)
                    throw new InvalidOperationException("Невірний формат дерева Хафмана.");
                current = current.Right;
            }

            if (current.Left == null && current.Right == null)
            {
                result.Append(current.Symbol);
                current = root;
            }
        }

        return result.ToString();
    }

    public static void SaveDecodedText(string decodedText, string filePath)
    {
        File.WriteAllText(filePath, decodedText);
    }
    
    public static void SaveEncodedBinary(string encodedText, string filePath)
    {
        var bytes = new List<byte>();
        for (int i = 0; i < encodedText.Length; i += 8)
        {
            var byteString = encodedText.Substring(i, Math.Min(8, encodedText.Length - i));
            if (byteString.Length < 8)
            {
                byteString = byteString.PadRight(8, '0');
            }
            bytes.Add(Convert.ToByte(byteString, 2));
        }

        File.WriteAllBytes(filePath, bytes.ToArray());
    }

    public static string LoadEncodedBinary(string filePath)
    {
        var bytes = File.ReadAllBytes(filePath);
        var sb = new StringBuilder();

        foreach (var b in bytes)
        {
            sb.Append(Convert.ToString(b, 2).PadLeft(8, '0'));
        }

        return sb.ToString();
    }
}
