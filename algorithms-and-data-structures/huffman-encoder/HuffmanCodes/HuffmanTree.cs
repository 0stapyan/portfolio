namespace HuffmanCodes;

public class HuffmanTree
{
    public static HuffmanNode BuildTree(Dictionary<char, int> frequency)
    {
        var minHeap = new HuffmanNode.MinHeap();
        foreach (var key_value in frequency)
        {
            minHeap.Add(new HuffmanNode { Symbol = key_value.Key, Frequency = key_value.Value });
        }

        while (minHeap.Count > 1)
        {
            var left = minHeap.RemoveMin();
            var right = minHeap.RemoveMin();
            minHeap.Add(new HuffmanNode
            {
                Left = left,
                Right = right,
                Frequency = left.Frequency + right.Frequency
            });
        }

        return minHeap.RemoveMin();
    }
}