namespace ADHuffmanCode.Service.API;

public interface IHuffmanAlgorithmService {
    void BuildHuffmanTree(Dictionary<char,int> frequencyDict);
    string Encode(string text);
    string Decode(string text);
    Dictionary<char, string> GetHuffmanCodes();

}