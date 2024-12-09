namespace ADHuffmanCode.Service.API;

public interface ICompressService {
    string Compress(string path);
    string Decompress(string path);
}