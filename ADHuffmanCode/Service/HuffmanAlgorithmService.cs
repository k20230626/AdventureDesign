using System;
using System.Collections.Generic;
using System.Text;
using ADHuffmanCode.Message;
using ADHuffmanCode.Model;
using CommunityToolkit.Mvvm.Messaging;

namespace ADHuffmanCode.Service.API.API;

public class HuffmanAlgorithmService : IHuffmanAlgorithmService {
    private Dictionary<char, string> _huffmanCodes = new Dictionary<char, string>();
    private Node _root;

    private class Node : IComparable<Node> {
        public char Character { get; set; }
        public int Frequency { get; set; }
        public Node Left { get; set; }
        public Node Right { get; set; }

        public Node(char character, int frequency) {
            Character = character;
            Frequency = frequency;
        }

        public int CompareTo(Node other) {
            return Frequency.CompareTo(other.Frequency);
        }
    }

    public void BuildHuffmanTree(Dictionary<char, int> frequencyDict) {
        // 1. Count frequencies


        // 2. Create priority queue
        var priorityQueue = new PriorityQueue<Node, int>();
        foreach (var kvp in frequencyDict) {
            priorityQueue.Enqueue(new Node(kvp.Key, kvp.Value), kvp.Value);
        }

        // 3. Build Huffman Tree
        while (priorityQueue.Count > 1) {
            var left = priorityQueue.Dequeue();
            var right = priorityQueue.Dequeue();
            var combined = new Node('\0', left.Frequency + right.Frequency) {
                Left = left,
                Right = right
            };
            priorityQueue.Enqueue(combined, combined.Frequency);
        }

        _root = priorityQueue.Dequeue();

        // 4. Generate Huffman Codes
        GenerateHuffmanCodes(_root, "");
    }


    public string Encode(string text) {
        var frequencyDict = this.CountFrequencies(text);
        this.BuildHuffmanTree(frequencyDict);
        if (_root == null)
            throw new InvalidOperationException("Huffman Tree is not built.");

        var encodedText = new StringBuilder();
        foreach (var ch in text) {
            if (_huffmanCodes.ContainsKey(ch))
                encodedText.Append(_huffmanCodes[ch]);
            else
                throw new InvalidOperationException($"Character '{ch}' not found in Huffman Tree.");
        }
        
        var format = new ADHFFileFormat {
            FrequencyDict = frequencyDict,
            CompressedData = encodedText.ToString()
        };
        
        int originalSize = text.Length * 8; 
        int compressedSize = encodedText.Length;

        double compressionRatio = (1 - ((double)compressedSize / originalSize)) * 100;
        int savedSpace = originalSize - compressedSize;

        WeakReferenceMessenger.Default.Send<CompressInfoMessage>(new CompressInfoMessage(new CompressInfo(compressionRatio,savedSpace)));
        
        Debug.WriteLine($"Compression Ratio: {compressionRatio:F2}%");
        Debug.WriteLine($"Saved Space: {savedSpace} bits");
        
        return JsonSerializer.Serialize(format);
    }

    public string Decode(string encodedText) {
        try {
            _root = null;
            var data = JsonSerializer.Deserialize<ADHFFileFormat>(encodedText);
            this.BuildHuffmanTree(data.FrequencyDict);
            
            if (_root == null)
                throw new InvalidOperationException("Huffman Tree is not built.");

            var currentNode = _root;
            var decodedText = new StringBuilder();

            foreach (var bit in data.CompressedData) {
                currentNode = bit == '0' ? currentNode.Left : currentNode.Right;

                if (currentNode.Left == null && currentNode.Right == null) {
                    decodedText.Append(currentNode.Character);
                    currentNode = _root;
                }
            }

            return decodedText.ToString();
        }
        catch (Exception e) {
            return e.Message;
        }
    }

    public Dictionary<char, string> GetHuffmanCodes() {
        return _huffmanCodes;
    }

    private void GenerateHuffmanCodes(Node node, string code) {
        if (node == null)
            return;

        // Leaf node
        if (node.Left == null && node.Right == null) {
            _huffmanCodes[node.Character] = code;
        }

        GenerateHuffmanCodes(node.Left, code + "0");
        GenerateHuffmanCodes(node.Right, code + "1");
    }


    private Dictionary<char, int> CountFrequencies(string text) {
        if (string.IsNullOrEmpty(text))
            throw new ArgumentException("Text cannot be null or empty.");


        var frequencyDict = new Dictionary<char, int>();
        foreach (var ch in text) {
            if (!frequencyDict.ContainsKey(ch))
                frequencyDict[ch] = 0;
            frequencyDict[ch]++;
        }

        return frequencyDict;
    }
}