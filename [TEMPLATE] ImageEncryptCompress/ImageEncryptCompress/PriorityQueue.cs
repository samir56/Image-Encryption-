using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ImageEncryptCompress
{

        internal class PriorityQueue<T>
        {
            private readonly SortedDictionary<int, Queue<T>> _sortedDictionary = new SortedDictionary<int, Queue<T>>();

            public int Count { get; private set; }

            public void Enqueue(T item, int frequency)
            {
                ++Count;
                if (!_sortedDictionary.ContainsKey(frequency)) _sortedDictionary[frequency] = new Queue<T>();
                _sortedDictionary[frequency].Enqueue(item);
            }

            public T Dequeue()
            {
                --Count;
                var item = _sortedDictionary.First();
                if (item.Value.Count == 1) _sortedDictionary.Remove(item.Key);
                return item.Value.Dequeue();
            }
        }
        internal class HuffmanNode
        
        {
            public HuffmanNode Parent { get; set; }
            public HuffmanNode Left { get; set; }
            public HuffmanNode Right { get; set; }
            public byte Value { get; set; }
            public int freq { get; set; }
    
        }
        internal class HuffmanTree
        {
            private readonly HuffmanNode _root;

            public HuffmanTree(IEnumerable<KeyValuePair<byte, int>> counts)
            {
                var priorityQueue = new PriorityQueue<HuffmanNode>();

                foreach (KeyValuePair<byte, int> kvp in counts)
                {
                    priorityQueue.Enqueue(new HuffmanNode { Value = kvp.Key, freq = kvp.Value }, kvp.Value);
                }

                while (priorityQueue.Count > 1)
                {
                    HuffmanNode n1 = priorityQueue.Dequeue();
                    HuffmanNode n2 = priorityQueue.Dequeue();
                    var n3 = new HuffmanNode { Left = n1, Right = n2, freq = n1.freq + n2.freq };
                    n1.Parent = n3;
                    n2.Parent = n3;
                    priorityQueue.Enqueue(n3, n3.freq);
                }

                _root = priorityQueue.Dequeue();
            }

            public IDictionary<byte, string> CreateEncodings()
            {
                var encodings = new Dictionary<byte, string>();
                Encode(_root, "", encodings);
                return encodings;
            }

            private void Encode(HuffmanNode node, string path, IDictionary<byte, string> encodings)
            {
                if (node.Left != null)
                {
                    Encode(node.Left, path + "0", encodings);
                    Encode(node.Right, path + "1", encodings);
                }
                else
                {
                    encodings.Add(node.Value, path);
                }
            }
        }
    
}

