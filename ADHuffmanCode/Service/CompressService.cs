using System;
using System.IO;
using Maui.Plugins.PageResolver.Attributes;

namespace ADHuffmanCode.Service.API;

[Singleton]
public class CompressService : ICompressService{
    private readonly IHuffmanAlgorithmService _algorithm;
    private readonly IFileService _fileService;
    public CompressService(IHuffmanAlgorithmService algorithm, IFileService fileService) {
        this._algorithm = algorithm;
        this._fileService = fileService;
    }
    public string Compress(string path) {
        var text = _fileService.ReadFile(path);
        var encodedText = _algorithm.Encode(text);
        
        using (var fileStream = new FileStream(path.Split(".")[0] + ".adhf", FileMode.Create)) {
            using (var binaryWriter = new BinaryWriter(fileStream)) {
                // Write frequency dictionary
                binaryWriter.Write(_algorithm.CountFrequencies(text).Count);
                foreach (var kvp in _algorithm.CountFrequencies(text)) {
                    binaryWriter.Write(kvp.Key);
                    binaryWriter.Write(kvp.Value);
                }
                // Write compressed data as bits
                int bitBuffer = 0;
                int bitCount = 0;

                foreach (var bit in encodedText.ToString()) {
                    bitBuffer = (bitBuffer << 1) | (bit == '1' ? 1 : 0);
                    bitCount++;

                    // Write byte when buffer is full
                    if (bitCount == 8) {
                        binaryWriter.Write((byte)bitBuffer);
                        bitBuffer = 0;
                        bitCount = 0;
                    }
                }

                // Write remaining bits if any
                if (bitCount > 0) {
                    bitBuffer = bitBuffer << (8 - bitCount); // Padding remaining bits with 0
                    binaryWriter.Write((byte)bitBuffer);
                }
            }
        }
        
        return encodedText;
    }

    public string Decompress(string path) {
        var outStr = string.Empty;
        using (var fileStream = new FileStream(path, FileMode.Open)) {
            using (var binaryReader = new BinaryReader(fileStream)) {
                outStr = _algorithm.Decode(binaryReader);
            }
        }

        if(string.IsNullOrEmpty(outStr))
            throw new InvalidOperationException("Decompression failed.");

        _fileService.WriteFile(path.Split(".")[0] + ".txt",outStr);

        return outStr;
    }
}