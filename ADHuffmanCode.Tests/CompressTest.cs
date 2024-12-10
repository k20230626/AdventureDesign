using System.Diagnostics;
using ADHuffmanCode.Service;
using ADHuffmanCode.Service.API;
using ADHuffmanCode.Tests.Utils;
using ICSharpCode.SharpZipLib.BZip2;
using XZ.NET;
using ICSharpCode.SharpZipLib;
namespace ADHuffmanCode.Tests;

public class CompressTest {
	private readonly string DummyPath = Path.Combine(Path.GetTempPath(), "dummy.txt");
	private const int DummyDataLength = 1000000;
	private void preTask() {
		var dummyData = new GenerateDummyData(DummyDataLength).CreateDummyData();
		new FileService().WriteFile(DummyPath,dummyData);
	}
	
	[Fact]
	public void CompressTimeCompareTest() {
		preTask();
		var huffmanCompress = new CompressService(new HuffmanAlgorithmService(), new FileService());
		var stopwatch = new Stopwatch();
		
		//Huffman Compress
		stopwatch.Start();
		huffmanCompress.Compress(DummyPath);
		stopwatch.Stop();
		var huffmanCompressTime = stopwatch.ElapsedMilliseconds;
		
		stopwatch.Reset();
		
		//Huffman Decompress
		stopwatch.Start();
		huffmanCompress.Decompress(Path.Combine(Path.GetTempPath(), "dummy.adhf"));
		stopwatch.Stop();
		var bzip2Path = Path.Combine(Path.GetTempPath(), "dummy_bzip2.txt");
		stopwatch.Restart();
		
	}
}
