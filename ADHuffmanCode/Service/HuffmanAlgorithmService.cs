using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ADHuffmanCode.Message;
using ADHuffmanCode.Model;
using ADHuffmanCode.Service.API;
using CommunityToolkit.Mvvm.Messaging;

namespace ADHuffmanCode.Service;

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
        var priorityQueue = new PriorityQueue<Node, int>();
        foreach (var kvp in frequencyDict) {
            priorityQueue.Enqueue(new Node(kvp.Key, kvp.Value), kvp.Value);
        }
        
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

        int originalSize = text.Length * 8;
        int compressedSize = encodedText.Length;

        double compressionRatio = (1 - ((double)compressedSize / originalSize)) * 100;
        int savedSpace = originalSize - compressedSize;

        WeakReferenceMessenger.Default.Send<CompressInfoMessage>(
            new CompressInfoMessage(new CompressInfo(compressionRatio, savedSpace)));


        return encodedText.ToString();
    }
    
    public string Decode(BinaryReader reader) {
        int dictCount = reader.ReadInt32();
        var frequencyDict = new Dictionary<char, int>();
        for (int i = 0; i < dictCount; i++) {
            char character = reader.ReadChar();
            int frequency = reader.ReadInt32();
            frequencyDict[character] = frequency;
        }

        this.BuildHuffmanTree(frequencyDict);
        if (_root == null)
            throw new InvalidOperationException("Huffman Tree is not built.");

        var currentNode = _root;
        var decodedText = new StringBuilder();


        while (reader.BaseStream.Position < reader.BaseStream.Length) {
            byte byteRead = reader.ReadByte();
            for (int i = 7; i >= 0; i--) {
                bool bit = (byteRead & (1 << i)) != 0;
                currentNode = bit ? currentNode.Right : currentNode.Left;

                if (currentNode.Left == null && currentNode.Right == null) {
                    decodedText.Append(currentNode.Character);
                    currentNode = _root;
                }
            }
        }


        return decodedText.ToString();
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


    public Dictionary<char, int> CountFrequencies(string text) {
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