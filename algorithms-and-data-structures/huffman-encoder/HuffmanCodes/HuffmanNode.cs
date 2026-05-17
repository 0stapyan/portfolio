namespace HuffmanCodes;

public class HuffmanNode : IComparable<HuffmanNode>
{
    public char Symbol { get; set; }
    public int Frequency { get; set; }
    public HuffmanNode Left { get; set; }
    public HuffmanNode Right { get; set; }

    public int CompareTo(HuffmanNode other)
    {
        return Frequency.CompareTo(other.Frequency);
    }

    public class MinHeap
    {
        private List<HuffmanNode> nodes = new List<HuffmanNode>();
        public int Count => nodes.Count;

        public void Add(HuffmanNode node)
        {
            nodes.Add(node);
            HeapifyUp(nodes.Count - 1);
        }

        private void HeapifyUp(int nodesCount)
        {
            while (nodesCount > 0)
            {
                int parentNode = (nodesCount - 1) / 2;
                if (nodes[nodesCount].CompareTo(nodes[parentNode]) >= 0)
                    break;
                HuffmanNode current = nodes[nodesCount];
                nodes[nodesCount] = nodes[parentNode];
                nodes[parentNode] = current;
                nodesCount = parentNode;
            }
        }

        private void HeapifyDown(int index)
        {
            int last = nodes.Count - 1;
            while (index <= last)
            {
                int smallest = index;
                int leftChild = 2 * index + 1;
                int rightChild = 2 * index + 2;

                if (leftChild <= last && nodes[leftChild].CompareTo(nodes[smallest]) < 0)
                    smallest = leftChild;

                if (rightChild <= last && nodes[rightChild].CompareTo(nodes[smallest]) < 0)
                    smallest = rightChild;

                if (smallest == index)
                    break;
                HuffmanNode current = nodes[index];
                nodes[index] = nodes[smallest];
                nodes[smallest] = current;
                index = smallest;
            }
        }

        public HuffmanNode RemoveMin()
        {
            var minNode = nodes[0];
            nodes[0] = nodes[nodes.Count - 1];
            nodes.RemoveAt(nodes.Count - 1);
            HeapifyDown(0);
            return minNode;
        }
    }
}