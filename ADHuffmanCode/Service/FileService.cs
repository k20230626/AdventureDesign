using System;
using System.IO;
using ADHuffmanCode.Service.API;

namespace ADHuffmanCode.Service;

public class FileService : IFileService {
    public string ReadFile(string path)
    {
        if (string.IsNullOrEmpty(path))
            throw new ArgumentException("Path cannot be null or empty.", nameof(path));

        if (!File.Exists(path))
            throw new FileNotFoundException($"The file at path {path} does not exist.");

        try
        {
            return File.ReadAllText(path);
        }
        catch (Exception ex)
        {
            // Handle file read error
            throw new IOException($"Failed to read the file at path {path}.", ex);
        }
    }

    public void WriteFile(string path, string content)
    {
        if (string.IsNullOrEmpty(path))
            throw new ArgumentException("Path cannot be null or empty.", nameof(path));

        if (content == null)
            throw new ArgumentNullException(nameof(content), "Content cannot be null.");

        try
        {
            File.WriteAllText(path, content);
        }
        catch (Exception ex)
        {
            // Handle file write error
            throw new IOException($"Failed to write to the file at path {path}.", ex);
        }
    }

}