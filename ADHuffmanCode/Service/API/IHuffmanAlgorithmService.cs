namespace ADHuffmanCode.Service.API;

public interface IHuffmanAlgorithmService {
    void BuildHuffmanTree(Dictionary<char,int> frequencyDict);
    string Encode(string text);
    string Decode(BinaryReader reader);
    Dictionary<char, string> GetHuffmanCodes();
    Dictionary<char,int> CountFrequencies(string text);

}