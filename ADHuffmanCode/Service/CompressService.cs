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
        var content = _fileService.ReadFile(path);
        
        var outStr = _algorithm.Encode(content);
        
        _fileService.WriteFile(path.Split(".")[0] + ".adhf",outStr);
        
        return outStr;
    }

    public string Decompress(string path) {
        var content = _fileService.ReadFile(path);

        var outStr = _algorithm.Decode(content);

        _fileService.WriteFile(path.Split(".")[0] + ".txt",outStr);

        return outStr;
    }
}