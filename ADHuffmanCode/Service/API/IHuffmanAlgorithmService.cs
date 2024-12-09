using System.Collections.Generic;
using System.IO;

namespace ADHuffmanCode.Service.API;

public interface IHuffmanAlgorithmService {
    string Encode(string text);
    string Decode(BinaryReader reader);
    Dictionary<char, string> GetHuffmanCodes();
    Dictionary<char,int> CountFrequencies(string text);

}