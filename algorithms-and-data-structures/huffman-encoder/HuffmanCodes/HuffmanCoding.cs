namespace HuffmanCodes;

public class HuffmanCoding
{
    public static Dictionary<char, string> CreateCode(HuffmanNode root)
    {
        var codes = new Dictionary<char, string>();
        RecursiveCreateCode(root, "", codes);
        return codes;
    }

    private static void RecursiveCreateCode(HuffmanNode node, string code, Dictionary<char, string> codes)
    {
        if (node.Left == null && node.Right == null)
        {
            codes[node.Symbol] = code;
            return;
        }

        if (node.Left != null)
        {
            RecursiveCreateCode(node.Left, code + "0", codes);
        }
        if (node.Right != null)
        {
            RecursiveCreateCode(node.Right, code + "1", codes);
        }
    }
}